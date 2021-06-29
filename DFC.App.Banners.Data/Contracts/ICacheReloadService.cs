using DFC.App.Banners.Data.Models.CmsApiModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.Banners.Data.Contracts
{
    public interface ICacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);

        //Task<IList<ContentItemSummaryApiDataModel>?> GetSummaryListAsync();

        //Task ProcessSummaryListAsync(IList<ContentItemSummaryApiDataModel> summaryList, CancellationToken stoppingToken);

        //Task GetAndSaveItemAsync(ContentItemSummaryApiDataModel item, CancellationToken stoppingToken);
    }
}
