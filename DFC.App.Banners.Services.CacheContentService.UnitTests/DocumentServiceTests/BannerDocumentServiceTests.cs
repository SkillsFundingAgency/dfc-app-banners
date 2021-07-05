using DFC.App.Banners.Data.Models.ContentModels;
using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.DocumentServiceTests
{
    [Trait("Category", "Banner Document Service Unit Tests")]
    public class BannerDocumentServiceTests : BaseBannerDocumentServiceTests
    {
        [Theory]
        [InlineData(true)]
        public async Task BannerDocumentServiceDeleteAsyncDeleteByGuidId(bool documentServiceResponse)
        {
            // Arrage
            A.CallTo(() => FakeDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(documentServiceResponse);
            var service = BuildBannerDocumentService();

            // Act
            var result = await service.DeleteAsync(ContentIdForDelete);

            //Assert
            A.CallTo(() => FakeDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(result, documentServiceResponse);
        }

        [Fact]
        public async Task BannerDocumentServiceGetByIdAsyncReturnsPageBannerContentItemModels()
        {
            // Arrage
            A.CallTo(() => FakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(BuildValidContentItemModel());
            var service = BuildBannerDocumentService();

            // Act
            var result = await service.GetByIdAsync(ContentIdForUpdate, "/");

            //Assert
            A.CallTo(() => FakeDocumentService.GetByIdAsync(A<Guid>.Ignored,A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(result?.Id, BuildValidContentItemModel().Id);
        }

        [Fact]
        public async Task BannerDocumentServiceUpsertAsyncReturnsSuccess()
        {
            // Arrage
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            A.CallTo(() => FakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(expectedResult);
            var service = BuildBannerDocumentService();

            // Act
            var result = await service.UpsertAsync(BuildValidContentItemModel());

            //Assert
            A.CallTo(() => FakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(result, expectedResult);
        }
    }
}
