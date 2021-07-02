using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Enums;
using DFC.App.Banners.Data.Helpers;
using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class WebhooksService : IWebhooksService
    {
        private readonly ILogger<WebhooksService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentTypeMappingService contentTypeMappingService;
        private readonly IBannerDocumentService bannerCosmosRepository;


        public WebhooksService(
            ILogger<WebhooksService> logger,
            AutoMapper.IMapper mapper,
            ICmsApiService cmsApiService,
            IContentTypeMappingService contentTypeMappingService,
            IBannerDocumentService bannerCosmosRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.cmsApiService = cmsApiService;
            this.contentTypeMappingService = contentTypeMappingService;
            this.bannerCosmosRepository = bannerCosmosRepository;
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint)
        {
            if (string.IsNullOrEmpty(apiEndpoint))
            {
                return HttpStatusCode.Accepted;
            }

            contentTypeMappingService.AddMapping("Pagebanner", typeof(PageBannerContentItemApiDataModel));
            contentTypeMappingService.AddMapping("Banner", typeof(BannerContentItemApiDataModel));

            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:

                    //if (apiEndpoint.Contains(CmsContentKeyHelper.PageBannerTag, StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    return await DeleteContentAsync(contentId);
                    //}
                    //else
                    //{
                    //    throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    //}
                    return await DeleteContentAsync(contentId);

                case WebhookCacheOperation.CreateOrUpdate:

                    if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
                    {
                        throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    }

                    //if (apiEndpoint.Contains(CmsContentKeyHelper.PageBannerTag, StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    return await ProcessContentAsync(url);
                    //}
                    //else
                    //{
                    //    throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    //}

                    return await ProcessContentAsync(url);

                default:
                    logger.LogError($"Event Id: {eventId} got unknown cache operation - {webhookCacheOperation}");
                    return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Uri url)
        {
            var apiDataModel = await cmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(url);

            var pageBannerContentItemModel = mapper.Map<PageBannerContentItemModel>(apiDataModel);

            //var result = await bannerCosmosRepository.GetPagebannerUrisAsync("2e0e237d-bb0e-4c46-9534-94ce2b55f98d");

            if (pageBannerContentItemModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            if (!TryValidateModel(pageBannerContentItemModel))
            {
                return HttpStatusCode.BadRequest;
            }

            pageBannerContentItemModel.Banners = mapper.Map<List<BannerContentItemModel>>(apiDataModel?.ContentItems);

            var contentResult = await bannerCosmosRepository.UpsertAsync(pageBannerContentItemModel);
            return contentResult;
        }

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId)
        {
            var result = await bannerCosmosRepository.DeleteAsync(contentId);

            return result ? HttpStatusCode.OK : HttpStatusCode.NoContent;
        }

        public bool TryValidateModel(PageBannerContentItemModel? pageBannerContentItemModel)
        {
            _ = pageBannerContentItemModel ?? throw new ArgumentNullException(nameof(pageBannerContentItemModel));

            var validationContext = new ValidationContext(pageBannerContentItemModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(pageBannerContentItemModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {pageBannerContentItemModel.PageLocation} - {pageBannerContentItemModel.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }
    }
}
