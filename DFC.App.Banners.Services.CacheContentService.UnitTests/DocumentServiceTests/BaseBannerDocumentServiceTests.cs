using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.Azure.Documents;
using System;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.DocumentServiceTests
{
    public abstract class BaseBannerDocumentServiceTests
    {
        public BaseBannerDocumentServiceTests()
        {
            FakeCosmosDbConnection = A.Fake<CosmosDbConnection>();
            FakeDocumentClient = A.Fake<IDocumentClient>();
            FakeDocumentService = A.Fake<IDocumentService<PageBannerContentItemModel>>();
        }

        protected Guid ContentIdForUpdate { get; } = Guid.NewGuid();

        protected Guid ContentIdForDelete { get; } = Guid.NewGuid();

        protected CosmosDbConnection FakeCosmosDbConnection { get; }

        protected IDocumentClient FakeDocumentClient { get; }

        protected IDocumentService<PageBannerContentItemModel> FakeDocumentService { get; }

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

        protected PageBannerContentItemModel BuildValidContentItemModel()
        {
            var model = new PageBannerContentItemModel()
            {
                Id = ContentIdForUpdate,
                Etag = Guid.NewGuid().ToString(),
                Url = new Uri("https://localhost"),
                LastReviewed = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                LastCached = DateTime.UtcNow,
            };

            return model;
        }

        protected BannerDocumentService BuildBannerDocumentService()
        {
            return new BannerDocumentService(FakeDocumentService, FakeCosmosDbConnection, FakeDocumentClient);
        }
    }
}