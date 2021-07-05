using System;
using System.Collections.Generic;
using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.WebhooksServiceTests
{
    public abstract class BaseWebhooksServiceTests
    {
        protected const string EventTypePublished = "published";
        protected const string EventTypeDraft = "draft";
        protected const string EventTypeDraftDiscarded = "draft-discarded";
        protected const string EventTypeDeleted = "deleted";
        protected const string EventTypeUnpublished = "unpublished";

        protected BaseWebhooksServiceTests()
        {
            Logger = A.Fake<ILogger<WebhooksService>>();
            FakeEventHandlers = new List<IEventHandler>();
        }

        protected Guid ContentIdForCreate { get; } = Guid.NewGuid();

        protected Guid ContentIdForUpdate { get; } = Guid.NewGuid();

        protected Guid ContentIdForDelete { get; } = Guid.NewGuid();

        protected ILogger<WebhooksService> Logger { get; }

        protected AutoMapper.IMapper FakeMapper { get; }

        protected ICmsApiService FakeCmsApiService { get; }

        protected IBannerDocumentService FakeBannerDocumentService { get; }

        protected IContentTypeMappingService FakeContentTypeMappingService { get; }

        protected IList<IEventHandler> FakeEventHandlers { get; }

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

        protected WebhooksService BuildWebhooksService()
        {
            var service = new WebhooksService(Logger, FakeEventHandlers);

            return service;
        }
    }
}
