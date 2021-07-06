using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;
using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class WebhooksContentProcessor : IWebhookContentProcessor
    {
        private readonly ILogger<WebhooksContentProcessor> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentTypeMappingService contentTypeMappingService;
        private readonly IBannerDocumentService bannerDocumentService;

        public WebhooksContentProcessor(
            ILogger<WebhooksContentProcessor> logger,
            AutoMapper.IMapper mapper,
            ICmsApiService cmsApiService,
            IContentTypeMappingService contentTypeMappingService,
            IBannerDocumentService bannerDocumentService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.cmsApiService = cmsApiService;
            this.contentTypeMappingService = contentTypeMappingService;
            this.bannerDocumentService = bannerDocumentService;
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Uri url)
        {
            contentTypeMappingService.AddMapping(CmsContentKeyHelper.PageBannerTag, typeof(PageBannerContentItemApiDataModel));
            contentTypeMappingService.AddMapping(CmsContentKeyHelper.BannerTag, typeof(BannerContentItemApiDataModel));

            var apiDataModel = await cmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(url);

            var pageBannerContentItemModel = mapper.Map<PageBannerContentItemModel>(apiDataModel);

            if (pageBannerContentItemModel == null)
            {
                logger.LogInformation($"Page Banner Url: {url}, result {HttpStatusCode.NoContent}: No content found");
                return HttpStatusCode.NoContent;
            }

            if (!TryValidateModel(pageBannerContentItemModel))
            {
                return HttpStatusCode.BadRequest;
            }

            pageBannerContentItemModel.Banners = mapper.Map<List<BannerContentItemModel>>(apiDataModel?.ContentItems);

            var result = await bannerDocumentService.UpsertAsync(pageBannerContentItemModel);

            logger.LogInformation($"Page Banner Url: {url}, result {result}: Updated content for Page Banner");

            return result;
        }

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId)
        {
            var result = await bannerDocumentService.DeleteAsync(contentId);

            logger.LogInformation($"Page Banner event Id: {contentId}, result {result}: Deleted content for Page Banner");

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
