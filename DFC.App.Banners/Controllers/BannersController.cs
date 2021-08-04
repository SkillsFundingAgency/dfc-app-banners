using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.Extensions;
using DFC.App.Banners.ViewModels;
using DFC.Compui.Cosmos.Contracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Controllers
{
    [Route("banners")]
    public class BannersController : Controller
    {
        public const string RegistrationPath = "banners";
        private const int CacheDurationInSeconds = 10;

        private readonly ILogger<BannersController> logger;
        private readonly IMapper mapper;
        private readonly IDocumentService<PageBannerContentItemModel> documentService;
        private readonly IMemoryCache memoryCache;

        public BannersController(
            ILogger<BannersController> logger,
            IMapper mapper,
            IDocumentService<PageBannerContentItemModel> documentService,
            IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.documentService = documentService;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> IndexAsync()
        {
            var viewModel = new IndexViewModel()
            {
                LocalPath = $"{RegistrationPath}/document",
                Documents = new List<IndexDocumentViewModel>(),
            };

            var documents = await GetAllDocuments();

            if (documents?.Any() == true)
            {
                var docs = documents.OrderBy(o => o.PageLocation)
                    .Select(a => new IndexDocumentViewModel
                    {
                        PageLocation = a.PageLocation,
                        PageName = a.PageName,
                    });

                viewModel.Documents.AddRange(docs);

                logger.LogInformation($"{nameof(Index)} has succeeded");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("document/{**path}")]
        public async Task<IActionResult> DocumentAsync(string? path = "/")
        {
            var pageBannerContentItemModel = await GetBannersAsync(path ?? "/");

            if (pageBannerContentItemModel?.Any() is true)
            {
                var document = mapper.Map<PageBannerViewModel>(pageBannerContentItemModel.First());
                logger.LogInformation($"{nameof(GetBannersAsync)} has succeeded");

                return this.NegotiateContentResult(document);
            }

            logger.LogWarning($"{nameof(GetBannersAsync)} has returned no results for path {path}");
            return NoContent();
        }

        [HttpGet]
        [Route("body/{**path}")]
        public async Task<IActionResult> BodyAsync(string? path = "/")
        {
            var pageBannerContentItemModel = await GetBannersAsync(path ?? "/");

            if (pageBannerContentItemModel?.Any() is true)
            {
                var document = mapper.Map<PageBannerViewModel>(pageBannerContentItemModel.First());
                logger.LogInformation($"{nameof(GetBannersAsync)} has succeeded");

                return this.NegotiateContentResult(document.Banners);
            }

            logger.LogWarning($"{nameof(GetBannersAsync)} has returned no results for path {path}");
            return NoContent();
        }

        private static string BuildCacheKey(string path)
        {
            return $"{nameof(BannersController)}_{path}";
        }

        private async Task<IEnumerable<PageBannerContentItemModel>?> GetAllDocuments()
        {
            var cacheKey = BuildCacheKey(nameof(GetAllDocuments));

            if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<PageBannerContentItemModel>? content))
            {
                content = await documentService.GetAllAsync();
                memoryCache.Set(cacheKey, content, TimeSpan.FromSeconds(CacheDurationInSeconds));
            }

            return content;
        }

        private async Task<IEnumerable<PageBannerContentItemModel>?> GetDocuments(string partitionKey)
        {
            var cacheKey = BuildCacheKey(partitionKey);

            if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<PageBannerContentItemModel>? content))
            {
                content = await documentService.GetAllAsync(partitionKey);
                memoryCache.Set(cacheKey, content, TimeSpan.FromSeconds(CacheDurationInSeconds));
            }

            return content;
        }

        private async Task<IEnumerable<PageBannerContentItemModel>> GetBannersAsync(string path)
        {
            if (!path.StartsWith('/'))
            {
                path = $"/{path}";
            }

            if (!path.Equals("/"))
            {
                path = path.TrimEnd('/');
            }

            var banners = await GetDocuments(path);

            if (banners?.Any() is true || string.IsNullOrWhiteSpace(path) || path.Equals("/"))
            {
                return banners ?? Array.Empty<PageBannerContentItemModel>();
            }

            var parentPath = path.Substring(0, path.LastIndexOf('/'));
            return await GetBannersAsync(parentPath);
        }
    }
}