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
    public class PagesController : Controller
    {
        public const string BradcrumbTitle = "Sample";
        public const string RegistrationPath = "sample";
        public const string LocalPath = "pages";
        public const string DefaultPageTitleSuffix = BradcrumbTitle + " | National Careers Service";
        public const string PageTitleSuffix = " | " + DefaultPageTitleSuffix;

        private readonly ILogger<PagesController> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IDocumentService<SharedContentItemModel> sharedContentItemDocumentService;

        public PagesController(
            ILogger<PagesController> logger,
            IMapper mapper,
            IDocumentService<SharedContentItemModel> sharedContentItemDocumentService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.sharedContentItemDocumentService = sharedContentItemDocumentService;
        }

        [HttpGet]
        [Route("/")]
        [Route("pages")]
        public async Task<IActionResult> Index()
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

        [HttpGet]
        [Route("pages/{documentId}/document")]
        public async Task<IActionResult> Document(Guid documentId)
        {
            var sharedContentItemModel = await sharedContentItemDocumentService.GetByIdAsync(documentId);

            if (sharedContentItemModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(sharedContentItemModel);
                var breadcrumbItemModel = mapper.Map<BreadcrumbItemModel>(sharedContentItemModel);

                viewModel.Breadcrumb = BuildBreadcrumb(LocalPath, breadcrumbItemModel);

                logger.LogInformation($"{nameof(Document)} has succeeded for: {documentId}");

                return this.NegotiateContentResult(viewModel);
            }

            logger.LogWarning($"{nameof(Document)} has returned no content for: {documentId}");

            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/htmlhead")]
        public async Task<IActionResult> HtmlHead(string article)
        {
            logger.LogWarning($"{nameof(HtmlHead)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(string article)
        {
            logger.LogWarning($"{nameof(Breadcrumb)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/bodytop")]
        public async Task<IActionResult> BodyTop(string article)
        {
            logger.LogWarning($"{nameof(BodyTop)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/body")]
        public async Task<IActionResult> Body(string article)
        {
            logger.LogWarning($"{nameof(Body)} has returned not found for: {article}");

            return NotFound();
        }

        [HttpGet]
        [Route("pages/{article}/bodyfooter")]
        public async Task<IActionResult> BodyFooter(string article)
        {
            logger.LogWarning($"{nameof(BodyFooter)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/herobanner")]
        public async Task<IActionResult> HeroBanner(string article)
        {
            logger.LogWarning($"{nameof(HeroBanner)} has returned no content for: {article}");

            return NoContent();
        }

        private static BreadcrumbViewModel BuildBreadcrumb(string segmentPath, BreadcrumbItemModel? breadcrumbItemModel)
        {
            const string slash = "/";
            var viewModel = new BreadcrumbViewModel
            {
                Breadcrumbs = new List<BreadcrumbItemViewModel>()
                {
                    new BreadcrumbItemViewModel()
                    {
                        Route = slash,
                        Title = "Home",
                    },
                },
            };

            if (breadcrumbItemModel?.Title != null && !string.IsNullOrWhiteSpace(breadcrumbItemModel.Route))
            {
                var articlePathViewModel = new BreadcrumbItemViewModel
                {
                    Route = slash + segmentPath,
                    Title = breadcrumbItemModel.Title,
                };

                viewModel.Breadcrumbs.Add(articlePathViewModel);
            }

            viewModel.Breadcrumbs.Last().AddHyperlink = false;

            return viewModel;
        }
    }
}