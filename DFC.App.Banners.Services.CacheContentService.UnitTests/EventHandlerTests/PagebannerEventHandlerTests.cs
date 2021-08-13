using System;
using System.Net;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;
using DFC.App.Banners.Data.Models.ContentModels;

using FakeItEasy;

using Microsoft.Extensions.Logging;

using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.EventHandlerTests
{
    [Trait("Category", "Pagebanner Event Handler Unit Tests")]
    public class PagebannerEventHandlerTests
    {
        private readonly ILogger<PageBannerEventHandler> logger;
        private readonly IBannersCacheReloadService fakeBannersCacheReloadService;
        private readonly IBannerDocumentService fakeBannerDocumentService;

        public PagebannerEventHandlerTests()
        {
            logger = A.Fake<ILogger<PageBannerEventHandler>>();
            fakeBannersCacheReloadService = A.Fake<IBannersCacheReloadService>();
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
            var pagebannerEventHandler = new PageBannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);
            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(pageBannerContentItemModel);
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await pagebannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

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
            var pagebannerEventHandler = new PageBannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(pageBannerContentItemModel);
            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(true);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await pagebannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task PagebannerEventHandlerDeleteContentAsyncForDeleteReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var contentId = Guid.NewGuid();

            var pagebannerEventHandler = new PageBannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);
            A.CallTo(() => fakeBannersCacheReloadService.DeletePageBannerContentAsync(A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await pagebannerEventHandler.DeleteContentAsync(contentId);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.DeletePageBannerContentAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void PagebannerEventHandlerProcessTypeReturnsCorrectValue()
        {
            // Arrange
            var pagebannerEventHandler = new PageBannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            // Assert
            Assert.Equal(pagebannerEventHandler.ProcessType, CmsContentKeyHelper.PageBannerTag);
        }
    }
}
