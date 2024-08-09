using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DFC.App.Banners.Extensions;
using DFC.App.Banners.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBanner;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AppConstants = DFC.Common.SharedContent.Pkg.Netcore.Constant.ApplicationKeys;

namespace DFC.App.Banners.Controllers
{
    [Route("banners")]
    public class BannersController : Controller
    {
        public const string RegistrationPath = "banners";
        private const string BaseUrlAppSettings = "Cms:NcsBaseUrl";
        private const string ExpiryAppSettings = "Cms:Expiry";
        private readonly ILogger<BannersController> logger;
        private readonly IMapper mapper;
        private readonly ISharedContentRedisInterface sharedContentRedis;
        private readonly IConfiguration configuration;
        private readonly string baseUrl;
        private string status;
        private double expiryInHours = 4;

        public BannersController(
            ILogger<BannersController> logger,
            IMapper mapper,
            ISharedContentRedisInterface sharedContentRedis,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.sharedContentRedis = sharedContentRedis;
            this.configuration = configuration;
            this.baseUrl = GetBaseUrl();
            status = configuration.GetSection("ContentMode:ContentMode").Get<string>() ?? "PUBLISHED";

            string expiryAppString = this.configuration.GetSection(ExpiryAppSettings).Get<string>();
            if (double.TryParse(expiryAppString, out var expiryAppStringParseResult))
            {
                expiryInHours = expiryAppStringParseResult;
            }
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

            if (string.IsNullOrEmpty(status))
            {
                status = "PUBLISHED";
            }

            var documents = await sharedContentRedis.GetDataAsyncWithExpiry<PageBannerResponse>(AppConstants.AllPageBanners, status, expiryInHours);
            var pageBanners = documents.PageBanner;

            if (pageBanners != null && pageBanners.Count != 0)
            {
                var docs = pageBanners.OrderBy(o => o.Banner.WebPageUrl)
                    .Select(a => new IndexDocumentViewModel
                    {
                        PageLocation = a.Banner.WebPageUrl.Replace(baseUrl, string.Empty),
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

            if (string.IsNullOrEmpty(status))
            {
                status = "PUBLISHED";
            }

            var pageBannerUrl = $"PageBanner/{baseUrl}{path}";
            var pageBannerContentItemModel = await sharedContentRedis.GetDataAsyncWithExpiry<PageBanner>(pageBannerUrl, status, expiryInHours);

            while (pageBannerContentItemModel == null)
            {
                if (pageBannerUrl == $"PageBanner/{baseUrl}")
                {
                    break;
                }

                pageBannerUrl = pageBannerUrl.Substring(0, pageBannerUrl.LastIndexOf('/'));
                pageBannerContentItemModel = await sharedContentRedis.GetDataAsyncWithExpiry<PageBanner>(pageBannerUrl, status, expiryInHours);
            }

            if (pageBannerContentItemModel != null && pageBannerContentItemModel.Banner != null)
            {
                pageBannerContentItemModel = TidyPageBannerFields(pageBannerContentItemModel);
                var document = mapper.Map<PageBannerViewModel>(pageBannerContentItemModel);
                logger.LogInformation($"{nameof(sharedContentRedis.GetDataAsyncWithExpiry)} has succeeded");

                return this.NegotiateContentResult(document);
            }

            logger.LogWarning($"{nameof(sharedContentRedis.GetDataAsyncWithExpiry)} has returned no results for path {path}");
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

            if (string.IsNullOrEmpty(status))
            {
                status = "PUBLISHED";
            }

            var pageBannerUrl = $"PageBanner/{baseUrl}{path}";
            var pageBannerContentItemModel = await sharedContentRedis.GetDataAsyncWithExpiry<PageBanner>(pageBannerUrl, status, expiryInHours);

            while (pageBannerContentItemModel == null)
            {
                if (pageBannerUrl == $"PageBanner/{baseUrl}")
                {
                    break;
                }

                pageBannerUrl = pageBannerUrl.Substring(0, pageBannerUrl.LastIndexOf('/'));
                pageBannerContentItemModel = await sharedContentRedis.GetDataAsyncWithExpiry<PageBanner>(pageBannerUrl, status, expiryInHours);
            }

            if (pageBannerContentItemModel != null && pageBannerContentItemModel.Banner != null)
            {
                pageBannerContentItemModel = TidyPageBannerFields(pageBannerContentItemModel);
                var document = mapper.Map<PageBannerViewModel>(pageBannerContentItemModel);
                logger.LogInformation($"{nameof(sharedContentRedis.GetDataAsyncWithExpiry)} has succeeded");

                return this.NegotiateContentResult(document.Banners);
            }

            logger.LogWarning($"{nameof(sharedContentRedis.GetDataAsyncWithExpiry)} has returned no results for path {path}");
            return NoContent();
        }

        private PageBanner TidyPageBannerFields(PageBanner? originalPageBanner)
        {
            var cleanPageBanner = originalPageBanner;
            var nodeIdLength = originalPageBanner.GraphSync.NodeId.Length;

            cleanPageBanner.GraphSync.NodeId = originalPageBanner.GraphSync.NodeId.Substring(nodeIdLength - 36);

            return cleanPageBanner;
        }

        private string GetBaseUrl()
        {
            var baseUrlAppSettings = configuration.GetValue<string>(BaseUrlAppSettings);
            if (baseUrlAppSettings != null && baseUrlAppSettings != string.Empty)
            {
                return baseUrlAppSettings.Remove(baseUrlAppSettings.Length - 1);
            }

            return string.Empty;
        }
    }
}