using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.Banners.Data.Contracts
{
    public interface ISharedContentCacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);
    }
}
