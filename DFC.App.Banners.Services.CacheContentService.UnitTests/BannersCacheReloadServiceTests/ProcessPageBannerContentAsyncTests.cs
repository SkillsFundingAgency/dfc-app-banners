using System;
using System.Net;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;

using FakeItEasy;

using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.BannersCacheReloadServiceTests
{
    [Trait("Category", "Banners Cache Reload Service ProcessPageBannerContentAsync Unit Tests")]
    public class ProcessPageBannerContentAsyncTests : BaseBannersCacheReloadServiceTests
    {
        [Fact]
        public async Task ProcessPageBannerContentAsyncForCreateReturnsSuccess()
        {
            // Arrange
            var expectedResponse = true;
            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
            var expectedValidContentItemModel = BuildValidContentItemModel();
            var url = new Uri("https://somewhere.com");
            var service = BuildBannersCacheReloadService();

            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => FakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            var result = await service.ProcessPageBannerContentAsync(url);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task ProcessPageBannerContentAsyncForUpdateReturnsSuccess()
        {
            // Arrange
            var expectedResponse = true;
            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
            var expectedValidContentItemModel = BuildValidContentItemModel();
            var url = new Uri("https://somewhere.com");
            var service = BuildBannersCacheReloadService();

            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => FakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessPageBannerContentAsync(url);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task ProcessPageBannerContentAsyncForUpdateReturnsNoContent()
        {
            // Arrange
            var expectedResponse = false;
            PageBannerContentItemApiDataModel? expectedValidContentItemApiDataModel = null;
            PageBannerContentItemModel? expectedValidContentItemModel = default;
            var url = new Uri("https://somewhere.com");
            var service = BuildBannersCacheReloadService();

            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel?>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);

            // Act
            var result = await service.ProcessPageBannerContentAsync(url);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}
