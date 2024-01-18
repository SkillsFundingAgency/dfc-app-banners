//using System;
//using System.Threading.Tasks;

//using FakeItEasy;

//using Xunit;

//namespace DFC.App.Banners.Services.CacheContentService.UnitTests.BannersCacheReloadServiceTests
//{
//    [Trait("Category", "Banners Cache Reload Service DeletePageBannerContentAsync Unit Tests")]
//    public class DeletePageBannerContentAsyncTests : BaseBannersCacheReloadServiceTests
//    {
//        [Fact]
//        public async Task DeletePageBannerContentAsyncReturnsNoContent()
//        {
//            // Arrange
//            const bool expectedResponse = false;
//            var service = BuildBannersCacheReloadService();

//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResponse);

//            // Act
//            var result = await service.DeletePageBannerContentAsync(ContentIdForDelete);

//            // Assert
//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

//            Assert.Equal(expectedResponse, result);
//        }

//        [Fact]
//        public async Task DeletePageBannerContentAsyncForDeleteReturnsSuccess()
//        {
//            // Arrange
//            bool expectedResponse = true;
//            var service = BuildBannersCacheReloadService();

//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResponse);

//            // Act
//            var result = await service.DeletePageBannerContentAsync(ContentIdForDelete);

//            // Assert
//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

//            Assert.Equal(expectedResponse, result);
//        }
//    }
//}
