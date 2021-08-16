using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;

using FakeItEasy;

using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.BannersCacheReloadServiceTests
{
    [Trait("Category", "Content Cache Reload Service Unit Tests")]
    public class ContentCacheReloadServiceTests : BaseBannersCacheReloadServiceTests
    {
        [Fact]
        public async Task ReloadWhenCancellationRequestedThenCmsApiAndDocumentServiceNotCalled()
        {
            //Arrange
            var cancellationToken = new CancellationToken(true);
            var cacheReloadService = BuildBannersCacheReloadService();

            //Act
            await cacheReloadService.Reload(cancellationToken);

            //Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadContentWhenCancellationRequestedThenCmsApiAndDocumentServiceNotCalled()
        {
            //Arrange
            var cancellationToken = new CancellationToken(true);
            var cacheReloadService = BuildBannersCacheReloadService();

            var dummySummary = BuildCmsApiSummaryItemModel();
            A.CallTo(() => FakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary });

            //Act
            await cacheReloadService.ReloadContent(cancellationToken);

            //Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadWhenSuccessfulThenReloadsItems()
        {
            //Arrange
            var dummyContentItem = new PageBannerContentItemApiDataModel();
            var dummySummary = BuildCmsApiSummaryItemModel();

            A.CallTo(() => FakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary });
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(dummyContentItem);
            var cacheReloadService = BuildBannersCacheReloadService();

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => FakeBannerDocumentService.PurgeAsync()).MustNotHaveHappened();
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReloadWhenContentApiReturnsNothingThenPurgeCache()
        {
            //Arrange
            var dummyContentItem = new PageBannerContentItemApiDataModel();

            A.CallTo(() => FakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { });
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(dummyContentItem);
            var cacheReloadService = BuildBannersCacheReloadService();

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => FakeBannerDocumentService.PurgeAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadWhenMultiplePageBannersThenCmsApiCalledMultipleTimes()
        {
            //Arrange
            var dummyContentItem = A.Dummy<PageBannerContentItemApiDataModel>();
            var dummySummary1 = BuildCmsApiSummaryItemModel("https://sample1.com");
            var dummySummary2 = BuildCmsApiSummaryItemModel("https://sample2.com");
            var dummySummary3 = BuildCmsApiSummaryItemModel("https://sample3.com");

            A.CallTo(() => FakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary1, dummySummary2, dummySummary3 });
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(dummyContentItem);

            var cacheReloadService = BuildBannersCacheReloadService();

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappened(3, Times.Exactly);
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappened(3, Times.Exactly);
        }

        [Fact]
        public async Task ReloadWhenGetSummaryReturnsNullOrEmptyThenGetItemsAndDocumentServiceNotCalled()
        {
            //Arrange
            A.CallTo(() => FakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { });
            var cacheReloadService = BuildBannersCacheReloadService();

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadWhenGetItemsReturnsNullThenDocumentServiceNotCalled()
        {
            //Arrange
            var dummySummary = BuildCmsApiSummaryItemModel();
            PageBannerContentItemApiDataModel? nullContentItem = null;

            A.CallTo(() => FakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary });
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(nullContentItem);
            var cacheReloadService = BuildBannersCacheReloadService();

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadExceptionThrown()
        {
            //Arrange
            var ex = new Exception("some exception");

            A.CallTo(() => FakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).ThrowsAsync(ex);

            var cacheReloadService = BuildBannersCacheReloadService();

            //Act + Assert
            var e = await Assert.ThrowsAsync<Exception>(async () => await cacheReloadService.Reload(CancellationToken.None));
            Assert.Equal(e.Message, ex.Message);

            //Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }
    }
}