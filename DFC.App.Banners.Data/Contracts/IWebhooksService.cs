using System;
using System.Net;
using System.Threading.Tasks;
using DFC.App.Banners.Data.Enums;

namespace DFC.App.Banners.Data.Contracts
{
    public interface IWebhooksService
    {
        Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid contentId, string apiEndpoint);
    }
}
