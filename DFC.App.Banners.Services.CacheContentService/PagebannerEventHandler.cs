using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class PagebannerEventHandler : IEventHandler
    {
        private readonly IBannersCacheReloadService bannersCacheReloadService;

        public PagebannerEventHandler(IBannersCacheReloadService bannersCacheReloadService)
        {
            this.bannersCacheReloadService = bannersCacheReloadService;
        }

        public string ProcessType => CmsContentKeyHelper.PageBannerTag;

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId, Uri url)
        {
            return await bannersCacheReloadService.DeletePageBannerContentAsync(contentId);
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Guid contentId, Uri url)
        {
            return await bannersCacheReloadService.ProcessPageBannerContentAsync(url);
        }
    }
}
