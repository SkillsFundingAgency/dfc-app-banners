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
        private readonly IWebhookContentProcessor webhookContentProcessor;
        private readonly IBannerDocumentService bannerDocumentService;
        private readonly ILogger<BannerEventHandler> logger;

        public BannerEventHandler(IWebhookContentProcessor webhookContentProcessor, IBannerDocumentService bannerDocumentService, ILogger<BannerEventHandler> logger)
        {
            this.webhookContentProcessor = webhookContentProcessor;
            this.bannerDocumentService = bannerDocumentService;
            this.logger = logger;
        }

        public string ProcessType => DependencyInjectionKeyHelpers.BannerEventHandler;

        public async Task<HttpStatusCode> DeleteContentAsync(Guid contentId, Uri url)
        {
            return await ProcessBannerContent(contentId, url);
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Guid contentId, Uri url)
        {
            return await ProcessBannerContent(contentId, url);
        }

        private async Task<HttpStatusCode> ProcessBannerContent(Guid contentId, Uri url)
        {
            var pagebannerUrls = await bannerDocumentService.GetPagebannerUrlsAsync(contentId.ToString());

            if (!pagebannerUrls.Any())
            {
                return HttpStatusCode.Accepted;
            }

            try
            {
                var result = await Task.WhenAll(pagebannerUrls.Select(x => webhookContentProcessor.ProcessContentAsync(x)));
                return result.All(x => x == HttpStatusCode.OK) ? HttpStatusCode.OK : HttpStatusCode.NoContent;
            }
            catch (AggregateException exception)
            {
                exception.Flatten().InnerExceptions.ToList().ForEach(x => logger.LogError($"Failed to refresh cache for {url} : {exception.Flatten().Message}"));
                return HttpStatusCode.NoContent;
            }
        }
    }
}