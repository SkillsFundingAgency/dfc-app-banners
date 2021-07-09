using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.Services.CacheContentService.UnitTests.DocumentServiceTests.TestDoubles;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.DocumentServiceTests
{
    public abstract class BaseBannerDocumentServiceTests
    {
        public BaseBannerDocumentServiceTests()
        {
            FakeDocumentClient = A.Fake<IDocumentClient>();
            FakeDocumentService = A.Fake<IDocumentService<PageBannerContentItemModel>>();
            FakeDocumentQuery = A.Fake<IFakeDocumentQuery<IEnumerable<string>>>();
        }

        protected Guid ContentIdForUpdate { get; } = Guid.NewGuid();

        protected Guid ContentIdForDelete { get; } = Guid.NewGuid();

        protected IDocumentClient FakeDocumentClient { get; }

        protected IDocumentService<PageBannerContentItemModel> FakeDocumentService { get; }

        protected IFakeDocumentQuery<IEnumerable<string>> FakeDocumentQuery { get; }

        protected static PageBannerContentItemApiDataModel BuildValidContentItemApiDataModel()
        {
            var model = new PageBannerContentItemApiDataModel
            {
                Title = "an-article",
                Url = new Uri("https://localhost"),
                Published = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
            };

            return model;
        }

        protected PageBannerContentItemModel BuildValidPageBannerContentItemModel(string pagebannerUrl = "https://localhost")
        {
            var model = new PageBannerContentItemModel()
            {
                Id = ContentIdForUpdate,
                Etag = Guid.NewGuid().ToString(),
                Url = new Uri(pagebannerUrl),
                LastReviewed = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                LastCached = DateTime.UtcNow,
            };

            return model;
        }

        protected IEnumerable<PageBannerContentItemModel> BuildValidPageBannerContentItemModels(IEnumerable<string> pagebannerUrls)
        {
            return pagebannerUrls.Select(x => BuildValidPageBannerContentItemModel(x));
        }

        protected BannerDocumentService BuildBannerDocumentService()
        {
            var cosmosDbConnection = new CosmosDbConnection
            {
                DatabaseId = "dfc-app-banners",
                CollectionId = "banners",
            };
            return new BannerDocumentService(FakeDocumentService, cosmosDbConnection, FakeDocumentClient);
        }
    }
}