using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;

using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class BannersCacheReloadService : IBannersCacheReloadService
    {
        private readonly ILogger<BannersCacheReloadService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IBannerDocumentService bannerDocumentService;
        private readonly IApiCacheService apiCacheService;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentTypeMappingService contentTypeMappingService;

        public BannersCacheReloadService(
            ILogger<BannersCacheReloadService> logger,
            AutoMapper.IMapper mapper,
            IBannerDocumentService bannerDocumentService,
            IContentTypeMappingService contentTypeMappingService,
            IApiCacheService apiCacheService,
            ICmsApiService cmsApiService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.bannerDocumentService = bannerDocumentService;
            this.contentTypeMappingService = contentTypeMappingService;
            this.apiCacheService = apiCacheService;
            this.cmsApiService = cmsApiService;

            AddMapSettings();
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
            var pageBanners = await cmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>();
            if (pageBanners is null || pageBanners.Count < 1)
            {
                await bannerDocumentService.PurgeAsync();
                return;
            }

            var pageBannersContentItems = new List<PageBannerContentItemModel>(pageBanners.Count);
            foreach (var pageBanner in pageBanners)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload banners content cache cancelled");

                    return;
                }

                var contentItem = await GetPageBannerContentItemAsync(pageBanner.Url!);
                if (contentItem != null)
                {
                    pageBannersContentItems.Add(contentItem);
                }
            }

            var cachedDocuments = await bannerDocumentService.GetAllAsync();
            var docsToDeletetasks = cachedDocuments
                            .Where(pb => !pageBannersContentItems.Select(x => x.PageLocation).Contains(pb.PageLocation))
                            .Select(pb => bannerDocumentService.DeleteAsync(pb.Id));

            await Task.WhenAll(docsToDeletetasks);
        }

        public async Task<bool> ProcessPageBannerContentAsync(Uri url)
        {
            var item = await GetPageBannerContentItemAsync(url);
            return item != null;
        }

        public async Task<bool> DeletePageBannerContentAsync(Guid contentId)
        {
            var result = await bannerDocumentService.DeleteAsync(contentId);

            logger.LogInformation($"Page Banner event Id: {contentId}, result {result}: Deleted content for Page Banner");

            return result;
        }

        private async Task<PageBannerContentItemModel?> GetPageBannerContentItemAsync(Uri url)
        {
            var apiDataModel = await cmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(url);
            if (apiDataModel == null)
            {
                logger.LogInformation($"Page Banner Url: {url}, result {HttpStatusCode.NoContent}: No content found");
                return null;
            }
            else
            {
                var mappedContentItem = mapper.Map<PageBannerContentItemModel>(apiDataModel);
                var result = await bannerDocumentService.UpsertAsync(mappedContentItem);

                logger.LogInformation($"Page Banner Url: {url}, result {result}: Updated content for Page Banner");

                return mappedContentItem;
            }
        }

        private void AddMapSettings()
        {
            this.contentTypeMappingService.AddMapping("PageBanner", typeof(PageBannerContentItemApiDataModel));
            this.contentTypeMappingService.AddMapping("Banner", typeof(BannerContentItemApiDataModel));
        }
    }
}