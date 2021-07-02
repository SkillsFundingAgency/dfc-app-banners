using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;

using FakeItEasy;

using Microsoft.Extensions.Logging;

using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests
{
    public class BannersContentCacheReloadServiceTests
    {
        private readonly IMapper fakeMapper = A.Fake<IMapper>();
        private readonly IDocumentService<PageBannerContentItemModel> fakeDocumentService = A.Fake<IDocumentService<PageBannerContentItemModel>>();
        private readonly ICmsApiService fakeCmsApiService = A.Fake<ICmsApiService>();
        private readonly IApiCacheService fakeApiCacheService = A.Fake<IApiCacheService>();
        private readonly IContentTypeMappingService fakeContentTypeMappingService = A.Fake<IContentTypeMappingService>();

        [Fact]
        public async Task ReloadWhenCancellationRequestedThenCmsApiAndDocumentServiceNotCalled()
        {
            //Arrange
            var cancellationToken = new CancellationToken(true);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(cancellationToken);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadContentWhenCancellationRequestedThenCmsApiAndDocumentServiceNotCalled()
        {
            //Arrange
            var cancellationToken = new CancellationToken(true);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            var dummySummary = new CmsApiSummaryItemModel { Url = new Uri("https://sample.com") };
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary });

            //Act
            await cacheReloadService.ReloadContent(cancellationToken);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadWhenSuccessfulThenReloadsItems()
        {
            //Arrange
            var dummyContentItem = new PageBannerContentItemApiDataModel();
            var dummySummary = new CmsApiSummaryItemModel { Url = new Uri("https://sample.com") };

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary });
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(dummyContentItem);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReloadWhenMultiplePageBannersThenCmsApiCalledMultipleTimes()
        {
            //Arrange
            var dummyContentItem = A.Dummy<PageBannerContentItemApiDataModel>();
            var dummySummary1 = new CmsApiSummaryItemModel { Url = new Uri("https://sample1.com") };
            var dummySummary2 = new CmsApiSummaryItemModel { Url = new Uri("https://sample2.com") };
            var dummySummary3 = new CmsApiSummaryItemModel { Url = new Uri("https://sample3.com") };

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary1, dummySummary2, dummySummary3 });
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(dummyContentItem);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappened(3, Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappened(3, Times.Exactly);
        }

        [Fact]
        public async Task ReloadWhenGetSummaryReturnsNullOrEmptyThenGetItemsAndDocumentServiceNotCalled()
        {
            //Arrange
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { });
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadWhenGetItemsReturnsNullThenDocumentServiceNotCalled()
        {
            //Arrange
            var dummySummary = new CmsApiSummaryItemModel { Url = new Uri("https://sample.com") };
            PageBannerContentItemApiDataModel? nullContentItem = null;

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary });
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(nullContentItem);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadExceptionThrown()
        {
            //Arrange
            var ex = new Exception("some exception");

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).ThrowsAsync(ex);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act + Assert
            var e = await Assert.ThrowsAsync<Exception>(async () => await cacheReloadService.Reload(CancellationToken.None));
            Assert.Equal(e.Message, ex.Message);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }
    }
}
