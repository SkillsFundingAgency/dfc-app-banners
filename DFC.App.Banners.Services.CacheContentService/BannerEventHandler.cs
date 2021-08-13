using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class BannerEventHandler : IEventHandler
    {
        private readonly IBannersCacheReloadService bannersCacheReloadService;
        private readonly IBannerDocumentService bannerDocumentService;
        private readonly ILogger<BannerEventHandler> logger;

        public BannerEventHandler(IBannersCacheReloadService bannersCacheReloadService, IBannerDocumentService bannerDocumentService, ILogger<BannerEventHandler> logger)
        {
            this.bannersCacheReloadService = bannersCacheReloadService;
            this.bannerDocumentService = bannerDocumentService;
            this.logger = logger;
        }

        public string ProcessType => CmsContentKeyHelper.BannerTag;

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId)
        {
            return await ProcessBannerContent(contentId);
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Guid contentId, Uri url)
        {
            return await ProcessBannerContent(contentId);
        }

        private async Task<HttpStatusCode> ProcessBannerContent(Guid contentId)
        {
            var pagebannerUrls = await bannerDocumentService.GetPageBannerUrlsAsync(contentId.ToString());

            if (!pagebannerUrls.Any())
            {
                logger.LogInformation($"Banner content item Id:{contentId} - No Page Banners found");
                return HttpStatusCode.Accepted;
            }

            try
            {
                var result = await Task.WhenAll(pagebannerUrls.Select(x => bannersCacheReloadService.ProcessPageBannerContentAsync(x)));

                var renVal = result.Any(x => x == HttpStatusCode.BadRequest) ? HttpStatusCode.BadRequest : HttpStatusCode.OK;

                logger.LogInformation($"Banner content item Id: {contentId} : Updated page banners: {string.Join(",", pagebannerUrls)}");

                return renVal;
            }
            catch (AggregateException exception)
            {
                exception.Flatten().InnerExceptions.ToList().ForEach(x => logger.LogError($"Failed to refresh cache for {contentId} : {exception.Flatten().Message}"));
                return HttpStatusCode.BadRequest;
            }
        }
    }
}