using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Enums;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Banners.IntegrationTests.Fakes
{
    public class FakeWebhooksService : IWebhooksService
    {
        public Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint, string contentType)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}
