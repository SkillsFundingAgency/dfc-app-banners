using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.Extensions;
using DFC.App.Banners.Models;
using DFC.App.Banners.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Controllers
{
    [Route("banners")]
    public class BannersController : Controller
    {
        public const string RegistrationPath = "sample";
        public const string LocalPath = "pages";

        private readonly ILogger<BannersController> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IDocumentService<BannerContentItemModel> sharedContentItemDocumentService;

        public BannersController(
            ILogger<BannersController> logger,
            IMapper mapper,
            IDocumentService<BannerContentItemModel> sharedContentItemDocumentService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.sharedContentItemDocumentService = sharedContentItemDocumentService;
        }

        [HttpGet]
        [Route("/{path}")]
        public async Task<IActionResult> GetAsync(string path)
        {
            var viewModel = new IndexViewModel()
            {
                Path = LocalPath,
                Documents = new List<IndexDocumentViewModel>()
                {
                    new IndexDocumentViewModel { Title = HealthController.HealthViewCanonicalName },
                    new IndexDocumentViewModel { Title = SitemapController.SitemapViewCanonicalName },
                    new IndexDocumentViewModel { Title = RobotController.RobotsViewCanonicalName },
                },
            };
            var sharedContentItemModels = await sharedContentItemDocumentService.GetAllAsync();

            if (sharedContentItemModels != null)
            {
                var documents = from a in sharedContentItemModels.OrderBy(o => o.Title)
                                select mapper.Map<IndexDocumentViewModel>(a);

                viewModel.Documents.AddRange(documents);

                logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return this.NegotiateContentResult(viewModel);
        }
    }
}