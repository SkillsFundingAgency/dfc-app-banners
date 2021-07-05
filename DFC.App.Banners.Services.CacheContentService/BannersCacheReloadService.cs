using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DFC.App.Banners.Data.Contracts;
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
        private readonly IContentTypeMappingService contentTypeMappingService;
        private readonly IApiCacheService apiCacheService;
        private readonly ICmsApiService cmsApiService;

        public BannersCacheReloadService(
            ILogger<BannersCacheReloadService> logger,
            AutoMapper.IMapper mapper,
            IDocumentService<PageBannerContentItemModel> documentService,
            IContentTypeMappingService contentTypeMappingService,
            IApiCacheService apiCacheService,
            ICmsApiService cmsApiService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.documentService = documentService;
            this.contentTypeMappingService = contentTypeMappingService;
            this.apiCacheService = apiCacheService;
            this.cmsApiService = cmsApiService;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Reload banners content cache started");

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload banners content cache cancelled");
                    return;
                }

                apiCacheService.StartCache();

                contentTypeMappingService.AddMapping("PageBanner", typeof(PageBannerContentItemApiDataModel));
                contentTypeMappingService.AddMapping("Banner", typeof(BannerContentItemApiDataModel));

                await ReloadContent(stoppingToken);
                logger.LogInformation("Reload banners content cache completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in banners content cache reload");
                throw;
            }
            finally
            {
                apiCacheService.StopCache();
            }
        }

        public async Task ReloadContent(CancellationToken stoppingToken)
        {
            await documentService.PurgeAsync();
            var pageBanners = await cmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>();
            if (pageBanners == null || pageBanners.Count < 1)
            {
                return;
            }

            foreach (var pageBanner in pageBanners)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload banners content cache cancelled");

                    return;
                }

                var apiDataModel = await cmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(pageBanner.Url!);

                if (apiDataModel == null)
                {
                    logger.LogError($"banners content: {pageBanner} not found in API response");
                }
                else
                {
                    var mappedContentItem = mapper.Map<PageBannerContentItemModel>(apiDataModel);
                    mappedContentItem.Banners = mapper.Map<List<BannerContentItemModel>>(apiDataModel.ContentItems);
                    await documentService.UpsertAsync(mappedContentItem);
                }
            }
        }
    }
}