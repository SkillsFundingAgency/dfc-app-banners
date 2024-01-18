//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//using DFC.App.Banners.Data.Contracts;
//using DFC.App.Banners.Data.Helpers;

//using Microsoft.Extensions.Logging;

//namespace DFC.App.Banners.Services.CacheContentService
//{
//    public class BannerEventHandler : IEventHandler
//    {
//        private readonly IBannersCacheReloadService bannersCacheReloadService;
//        private readonly IBannerDocumentService bannerDocumentService;
//        private readonly ILogger<BannerEventHandler> logger;

//        public BannerEventHandler(IBannersCacheReloadService bannersCacheReloadService, IBannerDocumentService bannerDocumentService, ILogger<BannerEventHandler> logger)
//        {
//            this.bannersCacheReloadService = bannersCacheReloadService;
//            this.bannerDocumentService = bannerDocumentService;
//            this.logger = logger;
//        }

//        public string ProcessType => CmsContentKeyHelper.BannerTag;

//        public Task<bool> DeleteContentAsync(Guid contentId) =>
//            ProcessBannerContent(contentId);

//        public Task<bool> ProcessContentAsync(Guid contentId, Uri url) =>
//            RefreshCache();

//        private async Task<bool> RefreshCache()
//        {
//            try
//            {
//                // Reload content to handle publishing unbpublished banners.
//                // local cache doesn't contain unpublished banners,
//                // which is why we have to do a full refresh.
//                await bannersCacheReloadService.ReloadContent(CancellationToken.None);
//                return true;
//            }
//            catch (AggregateException ex)
//            {
//                ex.Flatten().InnerExceptions.ToList().ForEach(x => logger.LogError(x, $"Failed to refresh cache. {x.Message}"));
//                return false;
//            }
//        }

//        private async Task<bool> ProcessBannerContent(Guid contentId)
//        {
//            var pagebannerUrls = await bannerDocumentService.GetPageBannerUrlsAsync(contentId.ToString());

//            if (!pagebannerUrls.Any())
//            {
//                logger.LogInformation($"Banner content item Id:{contentId} - No Page Banners found");
//                return true;
//            }

//            try
//            {
//                var result = await Task.WhenAll(pagebannerUrls.Select(url => bannersCacheReloadService.ProcessPageBannerContentAsync(url)));

//                logger.LogInformation($"Banner content item Id: {contentId} : Updated page banners: {string.Join(",", pagebannerUrls)}");

//                return result.All(x => x);
//            }
//            catch (AggregateException ex)
//            {
//                ex.Flatten().InnerExceptions.ToList().ForEach(x => logger.LogError(x, $"Failed to refresh cache for {contentId} : {x.Message}"));
//                return false;
//            }
//        }
//    }
//}