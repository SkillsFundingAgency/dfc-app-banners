using System;
using System.Threading;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;
using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;

using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class BannersCacheReloadService : ICacheReloadService
    {
        private readonly ILogger<BannersCacheReloadService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IDocumentService<PageBannerContentItemModel> documentService;
        private readonly ICmsApiService cmsApiService;

        public BannersCacheReloadService(ILogger<BannersCacheReloadService> logger, AutoMapper.IMapper mapper, IDocumentService<PageBannerContentItemModel> documentService, ICmsApiService cmsApiService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.documentService = documentService;
            this.cmsApiService = cmsApiService;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Reload shared content cache started");

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload shared content cache cancelled");
                    return;
                }

                await ReloadContent(stoppingToken);
                logger.LogInformation("Reload shared content cache completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in shared content cache reload");
                throw;
            }
        }

        public async Task ReloadContent(CancellationToken stoppingToken)
        {
            var contentItemKeys = SharedContentKeyHelper.GetSharedContentKeys();

            foreach (var key in contentItemKeys)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload shared content cache cancelled");

                    return;
                }

                var apiDataModel = await cmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>("pagebanners", key);

                if (apiDataModel == null)
                {
                    logger.LogError($"shared content: {key} not found in API response");
                }
                else
                {
                    var mappedContentItem = mapper.Map<PageBannerContentItemModel>(apiDataModel);

                    await documentService.UpsertAsync(mappedContentItem);
                }
            }
        }
    }
}