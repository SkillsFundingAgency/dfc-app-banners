using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.Extensions;
using DFC.App.Banners.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBanner;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
//using DFC.Compui.Cosmos.Contracts;

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
        private readonly ISharedContentRedisInterface sharedContentRedis;
        //private readonly IDocumentService<PageBannerContentItemModel> documentService;

        public BannersController(
            ILogger<BannersController> logger,
            IMapper mapper,
            ISharedContentRedisInterface sharedContentRedis)
            //IDocumentService<PageBannerContentItemModel> documentService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.sharedContentRedis = sharedContentRedis;
            //this.documentService = documentService;
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

            var documents = new List<PageBannerContentItemModel>(); // await documentService.GetAllAsync();

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
        public async Task<IActionResult> DocumentAsync(string? path)
        {
            if (path != null)
            {
                path = $"/{path}";
            }

            var pageBannerUrl = $"pagebanner/https://nationalcareers.service.gov.uk{path}";
            var pageBannerContentItemModel = await sharedContentRedis.GetDataAsync<PageBanner>(pageBannerUrl); //Array.Empty<PageBannerContentItemModel>(); //await GetBannersAsync(path ?? "/");

            if (pageBannerContentItemModel != null)
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
        public async Task<IActionResult> BodyAsync(string? path = "/")
        {
            if (path != null)
            {
                path = $"/{path}";
            }

            var pageBannerUrl = $"pagebanner/https://nationalcareers.service.gov.uk{path}";
            var pageBannerContentItemModel = await sharedContentRedis.GetDataAsync<PageBanner>($"pagebanner/https://nationalcareers.service.gov.uk/{path}"); //Array.Empty<PageBannerContentItemModel>(); // await GetBannersAsync(path ?? "/");

            if (pageBannerContentItemModel != null)
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


        //private async Task<IEnumerable<PageBannerContentItemModel>> GetBannersAsync(string path)
        //{
        //    if (!path.StartsWith('/'))
        //    {
        //        path = $"/{path}";
        //    }

        //    if (!path.Equals("/"))
        //    {
        //        path = path.TrimEnd('/');
        //    }

        //    var banners = await documentService.GetAsync(a => a.PartitionKey == path);

        //    if (banners?.Any() is true || string.IsNullOrWhiteSpace(path) || path.Equals("/"))
        //    {
        //        return banners ?? Array.Empty<PageBannerContentItemModel>();
        //    }

        //    var parentPath = path.Substring(0, path.LastIndexOf('/'));
        //    return await GetBannersAsync(parentPath);
        //}
    }
}