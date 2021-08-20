using System;
using System.Threading.Tasks;

namespace DFC.App.Banners.Data.Contracts
{
    public interface IEventHandler
    {
        public string ProcessType { get; }

        Task<bool> ProcessContentAsync(Guid contentId, Uri url);

        Task<bool> DeleteContentAsync(Guid contentId);
    }
}