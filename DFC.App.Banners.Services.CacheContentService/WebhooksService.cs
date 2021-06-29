using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Enums;
using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class WebhooksService : IWebhooksService
    {
        private readonly ILogger<WebhooksService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly ICmsApiService cmsApiService;
        private readonly IDocumentService<PageBannerContentItemModel> documentService;

        public WebhooksService(
            ILogger<WebhooksService> logger,
            AutoMapper.IMapper mapper,
            ICmsApiService cmsApiService,
            IDocumentService<PageBannerContentItemModel> documentService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.cmsApiService = cmsApiService;
            this.documentService = documentService;
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint)
        {
            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                    return await DeleteContentAsync(contentId);

                case WebhookCacheOperation.CreateOrUpdate:
                    if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
                    {
                        throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    }

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

            if (pageBannerContentItemModel == null)
            {
                return HttpStatusCode.NoContent;
            }

            if (!TryValidateModel(pageBannerContentItemModel))
            {
                return HttpStatusCode.BadRequest;
            }

            var contentResult = await documentService.UpsertAsync(pageBannerContentItemModel);

            return contentResult;
        }

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId)
        {
            var result = await documentService.DeleteAsync(contentId);

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
