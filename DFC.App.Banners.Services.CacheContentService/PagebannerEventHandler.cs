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
        private readonly IBannerDocumentService bannerDocumentService;
        private readonly ILogger<PagebannerEventHandler> logger;
        private readonly IBannersCacheReloadService bannersCacheReloadService;

        public PagebannerEventHandler(IBannersCacheReloadService bannersCacheReloadService, IBannerDocumentService bannerDocumentService, ILogger<PagebannerEventHandler> logger)
        {
            this.bannersCacheReloadService = bannersCacheReloadService;
            this.bannerDocumentService = bannerDocumentService;
            this.logger = logger;
        }

        public string ProcessType => CmsContentKeyHelper.PageBannerTag;

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId, Uri url)
        {
            return await bannersCacheReloadService.DeletePageBannerContentAsync(contentId);
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Guid contentId, Uri url)
        {
            var pageBanner = await bannerDocumentService.GetByIdAsync(contentId);

            if (pageBanner != null)
            {
                var deleteResponse = await bannerDocumentService.DeleteAsync(contentId);
                logger.LogInformation($"Page Banner contentItem Id: {contentId}, result {deleteResponse}: Deleted content for Page Banner");
            }

            return await bannersCacheReloadService.ProcessPageBannerContentAsync(url);
        }
    }
}