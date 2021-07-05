using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Enums;
using DFC.App.Banners.Data.Helpers;
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class WebhooksService : IWebhooksService
    {
        private readonly ILogger<WebhooksService> logger;
        private readonly IEnumerable<IEventHandler> eventHandlers;

        public WebhooksService(
            ILogger<WebhooksService> logger,
            IEnumerable<IEventHandler> eventHandlers)
        {
            this.logger = logger;
            this.eventHandlers = eventHandlers;
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint, string contentType)
        {
            if (string.IsNullOrEmpty(apiEndpoint))
            {
                return HttpStatusCode.Accepted;
            }

            if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
            {
                throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
            }

            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:

                    return await GetEventMessageHandler(contentType).DeleteContentAsync(contentId, url);

                case WebhookCacheOperation.CreateOrUpdate:

                    return await GetEventMessageHandler(contentType).ProcessContentAsync(contentId, url);

                default:
                    logger.LogError($"Event Id: {eventId} got unknown cache operation - {webhookCacheOperation}");
                    return HttpStatusCode.BadRequest;
            }
        }

        private static void ValidateEventHandler(IEventHandler? handler, string eventHandlerName)
        {
            if (handler == null)
            {
                throw new InvalidOperationException($"No implementation for {eventHandlerName} Found");
            }
        }

        private IEventHandler GetEventMessageHandler(string contentType)
        {
            IEventHandler? handler;

            if (contentType == CmsContentKeyHelper.BannerTag)
            {
                handler = eventHandlers.FirstOrDefault(x => x.ProcessType == DependencyInjectionKeyHelpers.BannerEventHandler);

                ValidateEventHandler(handler, DependencyInjectionKeyHelpers.BannerEventHandler);
            }
            else
            {
                handler = eventHandlers.FirstOrDefault(x => x.ProcessType == DependencyInjectionKeyHelpers.PagebannerEventHandler);

                ValidateEventHandler(handler, DependencyInjectionKeyHelpers.PagebannerEventHandler);
            }

            return handler;
        }
    }
}
