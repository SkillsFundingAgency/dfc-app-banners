using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Banners.Data.Contracts
{
    public interface IWebhookContentProcessor
    {
        Task<HttpStatusCode> ProcessContentAsync(Uri url);

        Task<HttpStatusCode> DeleteContentAsync(Guid contentId);
    }
}
