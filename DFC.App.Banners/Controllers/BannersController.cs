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
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Controllers
{
    [Route("banners")]
    public class BannersController : Controller
    {
        public const string RegistrationPath = "banners";
        private readonly ILogger<BannersController> logger;
        private readonly IMapper mapper;
        private readonly IDocumentService<PageBannerContentItemModel> documentService;

        public BannersController(
            ILogger<BannersController> logger,
            IMapper mapper,
            IDocumentService<PageBannerContentItemModel> documentService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.documentService = documentService;
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

            var documents = await documentService.GetAllAsync();

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
            if (string.IsNullOrWhiteSpace(path))
            {
                return NoContent();
            }

            var pageBannerContentItemModel = await GetBannersAsync(path);

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
            if (string.IsNullOrWhiteSpace(path))
            {
                return NoContent();
            }

            var pageBannerContentItemModel = await GetBannersAsync(path);

            if (pageBannerContentItemModel?.Any() is true)
            {
                var document = mapper.Map<PageBannerViewModel>(pageBannerContentItemModel.First());
                logger.LogInformation($"{nameof(GetBannersAsync)} has succeeded");

                return this.NegotiateContentResult(document.Banners);
            }

            logger.LogWarning($"{nameof(GetBannersAsync)} has returned no results for path {path}");
            return NoContent();
        }

        private async Task<IEnumerable<PageBannerContentItemModel>> GetBannersAsync(string path)
        {
            if (!path.StartsWith('/'))
            {
                path = $"/{path}";
            }

            var banners = await documentService.GetAsync(a => a.PartitionKey == path);

            if (banners?.Any() is true || path.Equals("/"))
            {
                return banners ?? Array.Empty<PageBannerContentItemModel>();
            }

            var parentPath = path.Substring(0, path.LastIndexOf('/'));
            return await GetBannersAsync(parentPath);
        }
    }
}