using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;
using DFC.App.Banners.Data.Models.ContentModels;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.EventHandlerTests
{
    [Trait("Category", "Pagebanner Event Handler Unit Tests")]
    public class PagebannerEventHandlerTests
    {
        private readonly ILogger<BannerEventHandler> logger;
        private readonly IWebhookContentProcessor fakeWebhookContentProcessor;
        private readonly IBannerDocumentService fakeBannerDocumentService;

        public PagebannerEventHandlerTests()
        {
            logger = A.Fake<ILogger<BannerEventHandler>>();
            fakeWebhookContentProcessor = A.Fake<IWebhookContentProcessor>();
            fakeBannerDocumentService = A.Fake<IBannerDocumentService>();
        }

        [Fact]
        public async Task PagebannerEventHandlerProcessContentAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var url = new Uri("https://somewhere.com");
            var contentId = Guid.NewGuid();
            PageBannerContentItemModel? pageBannerContentItemModel = null;
            var pagebannerEventHandler = new PagebannerEventHandler(fakeWebhookContentProcessor, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(pageBannerContentItemModel);

            A.CallTo(() => fakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await pagebannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task PagebannerEventHandlerProcessContentAsyncCallDeleteAsyncForUpdateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var url = new Uri("https://somewhere.com");
            var contentId = Guid.NewGuid();
            PageBannerContentItemModel? pageBannerContentItemModel = new PageBannerContentItemModel()
            {
                Id = Guid.NewGuid(),
                Etag = Guid.NewGuid().ToString(),
                Url = new Uri("https://localhost"),
            };
            var pagebannerEventHandler = new PagebannerEventHandler(fakeWebhookContentProcessor, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(pageBannerContentItemModel);
            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(true);

            A.CallTo(() => fakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await pagebannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeWebhookContentProcessor.ProcessContentAsync(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task PagebannerEventHandlerDeleteContentAsyncForDeleteReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var url = new Uri("https://somewhere.com");
            var contentId = Guid.NewGuid();
            var pagebannerEventHandler = new PagebannerEventHandler(fakeWebhookContentProcessor, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeWebhookContentProcessor.DeleteContentAsync(A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await pagebannerEventHandler.DeleteContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeWebhookContentProcessor.DeleteContentAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void PagebannerEventHandlerProcessTypeReturnsCorrectValue()
        {
            // Arrange
            var pagebannerEventHandler = new PagebannerEventHandler(fakeWebhookContentProcessor, fakeBannerDocumentService, logger);

            // Assert
            Assert.Equal(pagebannerEventHandler.ProcessType, CmsContentKeyHelper.PageBannerTag);
        }
    }
}
