using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.BannersCacheReloadServiceTests
{
    [Trait("Category", "Banners Cache Reload Service DeletePageBannerContentAsync Unit Tests")]
    public class DeletePageBannerContentAsyncTests : BaseBannersCacheReloadServiceTests
    {
        [Fact]
        public async Task DeletePageBannerContentAsyncReturnsNoContent()
        {
            // Arrange
            const bool expectedResponse = false;
            const HttpStatusCode expectedResult = HttpStatusCode.NoContent;
            var service = BuildBannersCacheReloadService();

            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await service.DeletePageBannerContentAsync(ContentIdForDelete);

            // Assert
            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task DeletePageBannerContentAsyncForDeleteReturnsSuccess()
        {
            // Arrange
            const bool expectedResponse = true;
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            var service = BuildBannersCacheReloadService();

            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await service.DeletePageBannerContentAsync(ContentIdForDelete);

            // Assert
            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }
    }
}
