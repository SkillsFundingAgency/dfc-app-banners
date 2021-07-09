using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Helpers;
using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.EventHandlerTests
{
    [Trait("Category", "Pagebanner Event Handler Unit Tests")]
    public class PagebannerEventHandlerTests
    {
        private readonly IBannersCacheReloadService fakeBannersCacheReloadService;

        public PagebannerEventHandlerTests()
        {
            fakeBannersCacheReloadService = A.Fake<IBannersCacheReloadService>();
        }

        [Fact]
        public async Task PagebannerEventHandlerProcessContentAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var url = new Uri("https://somewhere.com");
            var contentId = Guid.NewGuid();
            var pagebannerEventHandler = new PagebannerEventHandler(fakeBannersCacheReloadService);

            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await pagebannerEventHandler.ProcessContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.ProcessPageBannerContentAsync(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task PagebannerEventHandlerDeleteContentAsyncForDeleteReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var url = new Uri("https://somewhere.com");
            var contentId = Guid.NewGuid();
            var pagebannerEventHandler = new PagebannerEventHandler(fakeBannersCacheReloadService);

            A.CallTo(() => fakeBannersCacheReloadService.DeletePageBannerContentAsync(A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await pagebannerEventHandler.DeleteContentAsync(contentId, url);

            // Assert
            A.CallTo(() => fakeBannersCacheReloadService.DeletePageBannerContentAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void PagebannerEventHandlerProcessTypeReturnsCorrectValue()
        {
            // Arrange
            var pagebannerEventHandler = new PagebannerEventHandler(fakeBannersCacheReloadService);

            // Assert
            Assert.Equal(pagebannerEventHandler.ProcessType, CmsContentKeyHelper.PageBannerTag);
        }
    }
}
