//using DFC.App.Banners.Data.Contracts;
//using DFC.App.Banners.Data.Models.CmsApiModels;
//using DFC.App.Banners.Data.Models.ContentModels;
//using DFC.Content.Pkg.Netcore.Data.Contracts;
//using FakeItEasy;
//using Microsoft.Extensions.Logging;
//using System;

//namespace DFC.App.Banners.Services.CacheContentService.UnitTests.WebhooksContentProcessorTests
//{
//    public abstract class BaseWebhooksContentProcessorTests
//    {
//        public BaseWebhooksContentProcessorTests()
//        {
//            Logger = A.Fake<ILogger<WebhooksContentProcessor>>();
//            FakeMapper = A.Fake<AutoMapper.IMapper>();
//            FakeCmsApiService = A.Fake<ICmsApiService>();
//            FakeContentTypeMappingService = A.Fake<IContentTypeMappingService>();
//            FakeBannerDocumentService = A.Fake<IBannerDocumentService>();
//        }

//        protected Guid ContentIdForCreate { get; } = Guid.NewGuid();

//        protected Guid ContentIdForUpdate { get; } = Guid.NewGuid();

//        protected Guid ContentIdForDelete { get; } = Guid.NewGuid();

//        protected ILogger<WebhooksContentProcessor> Logger { get; }

//        protected AutoMapper.IMapper FakeMapper { get; }

//        protected ICmsApiService FakeCmsApiService { get; }

//        protected IBannerDocumentService FakeBannerDocumentService { get; }

//        protected IContentTypeMappingService FakeContentTypeMappingService { get; }

//        protected static PageBannerContentItemApiDataModel BuildValidContentItemApiDataModel()
//        {
//            var model = new PageBannerContentItemApiDataModel
//            {
//                Title = "an-article",
//                Url = new Uri("https://localhost"),
//                Published = DateTime.UtcNow,
//                CreatedDate = DateTime.UtcNow,
//            };

//            return model;
//        }

//        protected PageBannerContentItemModel BuildValidContentItemModel()
//        {
//            var model = new PageBannerContentItemModel()
//            {
//                Id = ContentIdForUpdate,
//                Etag = Guid.NewGuid().ToString(),
//                Url = new Uri("https://localhost"),
//                LastReviewed = DateTime.UtcNow,
//                CreatedDate = DateTime.UtcNow,
//                LastCached = DateTime.UtcNow,
//            };

//            return model;
//        }

//        protected WebhooksContentProcessor BuildWebhooksContentProcessor()
//        {
//            return new WebhooksContentProcessor(Logger, FakeMapper, FakeCmsApiService, FakeContentTypeMappingService, FakeBannerDocumentService);
//        }
//    }
//}
