using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class PagebannerEventHandler : IEventHandler
    {
        private readonly IWebhookContentProcessor webhookContentProcessor;
        private readonly IBannerDocumentService bannerDocumentService;
        private readonly ILogger<BannerEventHandler> logger;

        public PagebannerEventHandler(IWebhookContentProcessor webhookContentProcessor, IBannerDocumentService bannerDocumentService, ILogger<BannerEventHandler> logger)
        {
            this.webhookContentProcessor = webhookContentProcessor;
            this.bannerDocumentService = bannerDocumentService;
            this.logger = logger;
        }

        public string ProcessType => CmsContentKeyHelper.PageBannerTag;

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId, Uri url)
        {
            return await webhookContentProcessor.DeleteContentAsync(contentId);
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Guid contentId, Uri url)
        {
            var pageBanner = await bannerDocumentService.GetByIdAsync(contentId);

            if (pageBanner != null)
            {
                var deleteResponse = await bannerDocumentService.DeleteAsync(contentId);
                logger.LogInformation($"Page Banner contentItem Id: {contentId}, result {deleteResponse}: Deleted content for Page Banner");
            }

            return await webhookContentProcessor.ProcessContentAsync(url);
        }
    }
}