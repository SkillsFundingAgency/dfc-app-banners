using System;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;

using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class PageBannerEventHandler : IEventHandler
    {
        private readonly IBannerDocumentService bannerDocumentService;
        private readonly ILogger<PageBannerEventHandler> logger;
        private readonly IBannersCacheReloadService bannersCacheReloadService;

        public PageBannerEventHandler(IBannersCacheReloadService bannersCacheReloadService, IBannerDocumentService bannerDocumentService, ILogger<PageBannerEventHandler> logger)
        {
            this.bannersCacheReloadService = bannersCacheReloadService;
            this.bannerDocumentService = bannerDocumentService;
            this.logger = logger;
        }

        public string ProcessType => CmsContentKeyHelper.PageBannerTag;

        public Task<bool> DeleteContentAsync(Guid contentId) =>
            bannersCacheReloadService.DeletePageBannerContentAsync(contentId);

        public async Task<bool> ProcessContentAsync(Guid contentId, Uri url)
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