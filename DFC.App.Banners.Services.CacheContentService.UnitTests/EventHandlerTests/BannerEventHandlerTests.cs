using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;

using FakeItEasy;

using FluentAssertions;

using Microsoft.Extensions.Logging;

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
            var expectedResponse = true;
            var url = new Uri("https://somewhere.com");
            var pagebannerUrls = new List<Uri>();

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPageBannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await bannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustNotHaveHappened();

            result.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForCreateReturnsSuccess()
        {
            // Arrange
            var expectedResponse = true;
            var url = new Uri("https://somewhere.com");
            var pagebannerUrls = new List<Uri> { new Uri("https://pagebanner1.com"), new Uri("https://pagebanner2.com") };

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPageBannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await bannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ReloadContent(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();

            result.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForCreateThrowsException()
        {
            // Arrange
            var expectedResponse = false;
            var url = new Uri("https://somewhere.com");
            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannersCacheReloadService.ReloadContent(A<CancellationToken>.Ignored)).Throws<AggregateException>();

            // Act
            var result = await bannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ReloadContent(A<CancellationToken>.Ignored)).MustHaveHappened();

            result.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForUpdateReturnsSuccess()
        {
            // Arrange
            var expectedResponse = true;
            var url = new Uri("https://somewhere.com");
            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            // Act
            var result = await bannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ReloadContent(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();

            result.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForDeleteReturnsTrue()
        {
            // Arrange
            var expectedResponse = true;
            var pagebannerUrls = new List<Uri> { new Uri("https://pagebanner1.com"), new Uri("https://pagebanner2.com") };

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPageBannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await bannerEventHandler.DeleteContentAsync(contentId);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustHaveHappenedTwiceExactly();

            result.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForDeleteWhenBannerCacehReloadServiceThrowsReturnsFalse()
        {
            // Arrange
            var expectedResponse = false;
            var pagebannerUrls = new List<Uri> { new Uri("https://pagebanner1.com"), new Uri("https://pagebanner2.com") };

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPageBannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Throws(() => new AggregateException("Something went wrong."));

            // Act
            var result = await bannerEventHandler.DeleteContentAsync(contentId);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();

            result.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForDeleteWhenNoPageBannerUrlsFoundReturnsTrue()
        {
            // Arrange
            var expectedResponse = true;
            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPageBannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(new List<Uri>());

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await bannerEventHandler.DeleteContentAsync(contentId);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustNotHaveHappened();
            result.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task BannerEventHandlerProcessContentAsyncForDeleteReturnsFalse()
        {
            // Arrange
            var pagebannerUrls = new List<Uri> { new Uri("https://pagebanner1.com"), new Uri("https://pagebanner2.com") };

            var contentId = Guid.NewGuid();
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            A.CallTo(() => fakeBannerDocumentService.GetPageBannerUrlsAsync(A<string>.Ignored, A<string?>.Ignored)).Returns(pagebannerUrls);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).ReturnsNextFromSequence(true, false);

            // Act
            var result = await bannerEventHandler.DeleteContentAsync(contentId);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustHaveHappenedTwiceExactly();

            result.Should().Be(false);
        }

        [Fact]
        public void BannerEventHandlerProcessTypeReturnsCorrectValue()
        {
            // Arrange
            var bannerEventHandler = new BannerEventHandler(fakeBannersCacheReloadService, fakeBannerDocumentService, logger);

            // Assert
            bannerEventHandler.ProcessType.Should().Be(CmsContentKeyHelper.BannerTag);
        }
    }
}
