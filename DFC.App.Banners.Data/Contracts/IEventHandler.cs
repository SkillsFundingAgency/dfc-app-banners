using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Banners.Data.Contracts
{
    public interface IEventHandler
    {
        public string ProcessType { get; }

        Task<HttpStatusCode> ProcessContentAsync(Guid contentId, Uri url);

        Task<HttpStatusCode> DeleteContentAsync(Guid contentId);
    }
}