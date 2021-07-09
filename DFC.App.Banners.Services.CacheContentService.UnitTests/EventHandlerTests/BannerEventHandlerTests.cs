using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.EventHandlerTests
{
    [Trait("Category", "Banner Event Handler Unit Tests")]
    public class BannerEventHandlerTests
    {
        private readonly ILogger<BannerEventHandler> logger;
        private readonly IBannersCacheReloadService fakeBannersCacheReloadService;
        private readonly IBannerDocumentService fakeBannerDocumentService;

        public BannerEventHandlerTests()
        {
            logger = A.Fake<ILogger<BannerEventHandler>>();
            fakeBannersCacheReloadService = A.Fake<IBannersCacheReloadService>();
            fakeBannerDocumentService = A.Fake<IBannerDocumentService>();
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForNoPagebannersReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Accepted;
            var url = new Uri("https://somewhere.com");
            var pagebannerUrls = new List<Uri>();

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPagebannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await bannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var url = new Uri("https://somewhere.com");
            var pagebannerUrls = new List<Uri> { new Uri("https://pagebanner1.com"), new Uri("https://pagebanner2.com") };

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPagebannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await bannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustHaveHappenedTwiceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForCreateThrowsException()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
            var url = new Uri("https://somewhere.com");
            var pagebannerUrls = new List<Uri> { new Uri("https://pagebanner1.com"), new Uri("https://pagebanner2.com") };

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPagebannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Throws<AggregateException>();

            // Act
            var result = await bannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForUpdateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var url = new Uri("https://somewhere.com");
            var pagebannerUrls = new List<Uri> { new Uri("https://pagebanner1.com"), new Uri("https://pagebanner2.com") };

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPagebannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await bannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustHaveHappenedTwiceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForUpdateReturnsUnsuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
            var url = new Uri("https://somewhere.com");
            var pagebannerUrls = new List<Uri> { new Uri("https://pagebanner1.com"), new Uri("https://pagebanner2.com") };

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPagebannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).ReturnsNextFromSequence(HttpStatusCode.BadRequest, HttpStatusCode.OK);

            // Act
            var result = await bannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustHaveHappenedTwiceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForDeleteReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var url = new Uri("https://somewhere.com");
            var pagebannerUrls = new List<Uri> { new Uri("https://pagebanner1.com"), new Uri("https://pagebanner2.com") };

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPagebannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await bannerEventHandler.DeleteContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustHaveHappenedTwiceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void BannerEventHandlerProcessTypeReturnsCorrectValue()
        {
            // Arrange
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            // Assert
            Assert.Equal(bannerEventHandler.ProcessType, CmsContentKeyHelper.BannerTag);
        }
    }
}
