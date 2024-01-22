using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using DFC.App.Banners.Extensions;
using DFC.App.Banners.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBanner;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
//using DFC.Compui.Cosmos.Contracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;

namespace DFC.App.Banners.Controllers
{
    [Route("banners")]
    public class BannersController : Controller
    {
        public const string RegistrationPath = "banners";
        private readonly ILogger<BannersController> logger;
        private readonly IMapper mapper;
        private readonly ISharedContentRedisInterface sharedContentRedis;

        public BannersController(
            ILogger<BannersController> logger,
            IMapper mapper,
            ISharedContentRedisInterface sharedContentRedis)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.sharedContentRedis = sharedContentRedis;
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

            var documents = await sharedContentRedis.GetDataAsync<PageBannerResponse>("PageBanners/All");
            var pageBanners = documents.PageBanner;

            if (pageBanners != null && pageBanners.Count != 0)
            {
                var docs = pageBanners.OrderBy(o => o.Banner.WebPageUrl)
                    .Select(a => new IndexDocumentViewModel
                    {
                        PageLocation = a.Banner.WebPageUrl.Replace("https://nationalcareers.service.gov.uk", ""),
                        PageName = a.Banner.WebPageName,
                    });

                viewModel.Documents.AddRange(docs);

                logger.LogInformation($"{nameof(Index)} has succeeded");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("document/{**path}")]
        public async Task<IActionResult> DocumentAsync(string? path)
        {
            if (path != null)
            {
                path = $"/{path}";
            }

            var pageBannerUrl = $"pagebanner/https://nationalcareers.service.gov.uk{path}";
            var pageBannerContentItemModel = await sharedContentRedis.GetDataAsync<PageBanner>(pageBannerUrl);

            if (pageBannerContentItemModel != null && pageBannerContentItemModel.Banner != null)
            {
                pageBannerContentItemModel = TidyPageBannerFields(pageBannerContentItemModel);
                var document = mapper.Map<PageBannerViewModel>(pageBannerContentItemModel);
                logger.LogInformation($"{nameof(sharedContentRedis.GetDataAsync)} has succeeded");

                return this.NegotiateContentResult(document);
            }

            logger.LogWarning($"{nameof(sharedContentRedis.GetDataAsync)} has returned no results for path {path}");
            return NoContent();
        }

        [HttpGet]
        [Route("body/{**path}")]
        public async Task<IActionResult> BodyAsync(string? path)
        {
            if (path != null)
            {
                path = $"/{path}";
            }

            var pageBannerUrl = $"pagebanner/https://nationalcareers.service.gov.uk{path}";
            var pageBannerContentItemModel = await sharedContentRedis.GetDataAsync<PageBanner>(pageBannerUrl);

            if (pageBannerContentItemModel != null && pageBannerContentItemModel.Banner != null)
            {
                pageBannerContentItemModel = TidyPageBannerFields(pageBannerContentItemModel);
                var document = mapper.Map<PageBannerViewModel>(pageBannerContentItemModel);
                logger.LogInformation($"{nameof(sharedContentRedis.GetDataAsync)} has succeeded");

                return this.NegotiateContentResult(document.Banners);
            }

            logger.LogWarning($"{nameof(sharedContentRedis.GetDataAsync)} has returned no results for path {path}");
            return NoContent();
        }

        private PageBanner TidyPageBannerFields(PageBanner? originalPageBanner)
        {
            var cleanPageBanner = originalPageBanner;
            var nodeIdLength = originalPageBanner.GraphSync.NodeId.Length;

            cleanPageBanner.GraphSync.NodeId = originalPageBanner.GraphSync.NodeId.Substring(nodeIdLength - 36);

            return cleanPageBanner;
        }
    }
}