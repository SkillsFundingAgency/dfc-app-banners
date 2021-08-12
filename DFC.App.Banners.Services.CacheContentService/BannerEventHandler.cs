using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;

using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class BannerEventHandler : IEventHandler
    {
        private readonly IWebhookContentProcessor webhookContentProcessor;
        private readonly IBannerDocumentService bannerDocumentService;
        private readonly ILogger<BannerEventHandler> logger;

        public BannerEventHandler(IWebhookContentProcessor webhookContentProcessor, IBannerDocumentService bannerDocumentService, ILogger<BannerEventHandler> logger)
        {
            this.webhookContentProcessor = webhookContentProcessor;
            this.bannerDocumentService = bannerDocumentService;
            this.logger = logger;
        }

        public string ProcessType => CmsContentKeyHelper.BannerTag;

        public Task<HttpStatusCode> DeleteContentAsync(Guid contentId)
        {
            return ProcessBannerContent(contentId);
        }

        public Task<HttpStatusCode> ProcessContentAsync(Guid contentId, Uri url)
        {
            return ProcessBannerContent(contentId);
        }

        private async Task<HttpStatusCode> ProcessBannerContent(Guid contentId)
        {
            // TODO: Fetch all page banners and see if this banner is referenced, update all page banners which are referencing it.
            var pagebannerUrls = await bannerDocumentService.GetPagebannerUrlsAsync(contentId.ToString());

            if (!pagebannerUrls.Any())
            {
                logger.LogInformation($"Banner content item Id:{contentId} - No Page Banners found");
                return HttpStatusCode.Accepted;
            }

            try
            {
                var result = await Task.WhenAll(pagebannerUrls.Select(webhookContentProcessor.ProcessContentAsync));

                logger.LogInformation($"Banner content item Id: {contentId} : Updated page banners: {string.Join(",", pagebannerUrls)}");

                return result.Any(x => x == HttpStatusCode.BadRequest) ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
            }
            catch (AggregateException exception)
            {
                exception.Flatten().InnerExceptions.ToList().ForEach(x => logger.LogError(x, $"Failed to refresh cache for {contentId} : {x.Message}"));
                return HttpStatusCode.BadRequest;
            }
        }
    }
}