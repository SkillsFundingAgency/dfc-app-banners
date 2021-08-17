using System;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.Banners.Data.Contracts
{
    public interface IBannersCacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);

        Task ReloadContent(CancellationToken stoppingToken);

        Task<bool> ProcessPageBannerContentAsync(Uri url);

        Task<bool> DeletePageBannerContentAsync(Guid contentId);
    }
}