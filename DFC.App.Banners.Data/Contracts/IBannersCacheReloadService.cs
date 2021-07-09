using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.Banners.Data.Contracts
{
    public interface IBannersCacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);

        Task ReloadContent(CancellationToken stoppingToken);

        Task<HttpStatusCode> ProcessPageBannerContentAsync(Uri url);

        Task<HttpStatusCode> DeletePageBannerContentAsync(Guid contentId);
    }
}