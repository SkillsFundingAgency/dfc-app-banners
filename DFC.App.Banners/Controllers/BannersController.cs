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
        public const string LocalPath = "banners";

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
        [Route("/{path}")]
        public async Task<IActionResult> GetAsync(string path)
        {
            var pageBannerContentItemModel = await documentService.GetAsync(a => a.PartitionKey == path);

            if (pageBannerContentItemModel != null)
            {
                var documents = pageBannerContentItemModel
                                .Select(a => mapper.Map<PageBannerViewModel>(a));

                logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return this.NegotiateContentResult(new PageBannerViewModel());
        }
    }
}