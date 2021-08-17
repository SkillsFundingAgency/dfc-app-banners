using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Enums;

using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class WebhooksService : IWebhooksService
    {
        private readonly ILogger<WebhooksService> logger;
        private readonly IEnumerable<IEventHandler> eventHandlers;

        public WebhooksService(ILogger<WebhooksService> logger, IEnumerable<IEventHandler> eventHandlers)
        {
            this.logger = logger;
            this.eventHandlers = eventHandlers;
        }

        public Task<bool> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint, string contentType)
        {
            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                    return GetEventMessageHandler(contentType).DeleteContentAsync(contentId);

                case WebhookCacheOperation.CreateOrUpdate:
                    if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
                    {
                        throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                    }

                    return GetEventMessageHandler(contentType).ProcessContentAsync(contentId, url);

                default:
                    logger.LogError($"Event Id: {eventId} got unknown cache operation - {webhookCacheOperation}");
                    return Task.FromResult(false);
            }
        }

        private IEventHandler GetEventMessageHandler(string contentType) =>
            eventHandlers.FirstOrDefault(x => x.ProcessType == contentType)
                ?? throw new InvalidOperationException($"No implementation for {contentType} Found");
    }
}
