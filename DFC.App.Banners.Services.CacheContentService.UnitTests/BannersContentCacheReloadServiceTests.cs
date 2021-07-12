using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests
{
    public class BannersContentCacheReloadServiceTests
    {
        private readonly IMapper fakeMapper = A.Fake<IMapper>();
        private readonly IBannerDocumentService fakeBannerDocumentService = A.Fake<IBannerDocumentService>();
        private readonly ICmsApiService fakeCmsApiService = A.Fake<ICmsApiService>();
        private readonly IApiCacheService fakeApiCacheService = A.Fake<IApiCacheService>();
        private readonly IContentTypeMappingService fakeContentTypeMappingService = A.Fake<IContentTypeMappingService>();
        private readonly Guid contentIdForDelete = Guid.NewGuid();
        private readonly Guid contentIdForUpdate = Guid.NewGuid();

        [Fact]
        public async Task ReloadWhenCancellationRequestedThenCmsApiAndDocumentServiceNotCalled()
        {
            //Arrange
            var cancellationToken = new CancellationToken(true);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(cancellationToken);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadContentWhenCancellationRequestedThenCmsApiAndDocumentServiceNotCalled()
        {
            //Arrange
            var cancellationToken = new CancellationToken(true);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            var dummySummary = new CmsApiSummaryItemModel { Url = new Uri("https://sample.com") };
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary });

            //Act
            await cacheReloadService.ReloadContent(cancellationToken);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadWhenSuccessfulThenReloadsItems()
        {
            //Arrange
            var dummyContentItem = new PageBannerContentItemApiDataModel();
            var dummySummary = new CmsApiSummaryItemModel { Url = new Uri("https://sample.com") };

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary });
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(dummyContentItem);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeBannerDocumentService.PurgeAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
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
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappened(3, Times.Exactly);
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappened(3, Times.Exactly);
        }

        [Fact]
        public async Task ReloadWhenGetSummaryReturnsNullOrEmptyThenGetItemsAndDocumentServiceNotCalled()
        {
            //Arrange
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { });
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadWhenGetItemsReturnsNullThenDocumentServiceNotCalled()
        {
            //Arrange
            var dummySummary = new CmsApiSummaryItemModel { Url = new Uri("https://sample.com") };
            PageBannerContentItemApiDataModel? nullContentItem = null;

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).Returns(new List<CmsApiSummaryItemModel>() { dummySummary });
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(nullContentItem);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ReloadExceptionThrown()
        {
            //Arrange
            var ex = new Exception("some exception");

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<CmsApiSummaryItemModel>()).ThrowsAsync(ex);
            var cacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            //Act + Assert
            var e = await Assert.ThrowsAsync<Exception>(async () => await cacheReloadService.Reload(CancellationToken.None));
            Assert.Equal(e.Message, ex.Message);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task DeletePageBannerContentAsyncReturnsNoContent()
        {
            // Arrange
            const bool expectedResponse = false;
            const HttpStatusCode expectedResult = HttpStatusCode.NoContent;
            var service = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await service.DeletePageBannerContentAsync(contentIdForDelete);

            // Assert
            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task DeletePageBannerContentAsyncForDeleteReturnsSuccess()
        {
            // Arrange
            const bool expectedResponse = true;
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            var service = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await service.DeletePageBannerContentAsync(contentIdForDelete);

            // Assert
            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessPageBannerContentAsyncForCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
            var expectedValidContentItemModel = BuildValidContentItemModel();
            var url = new Uri("https://somewhere.com");
            var service = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
            A.CallTo(() => fakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            var result = await service.ProcessPageBannerContentAsync(url);

            // Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task ProcessPageBannerContentAsyncForUpdateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
            var expectedValidContentItemModel = BuildValidContentItemModel();
            var url = new Uri("https://somewhere.com");
            var service = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
            A.CallTo(() => fakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessPageBannerContentAsync(url);

            // Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task ProcessPageBannerContentAsyncForUpdateReturnsNoContent()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
            PageBannerContentItemApiDataModel? expectedValidContentItemApiDataModel = null;
            PageBannerContentItemModel? expectedValidContentItemModel = default;
            var url = new Uri("https://somewhere.com");
            var service = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeBannerDocumentService, fakeContentTypeMappingService, fakeApiCacheService, fakeCmsApiService);

            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
            A.CallTo(() => fakeMapper.Map<PageBannerContentItemModel?>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);

            // Act
            var result = await service.ProcessPageBannerContentAsync(url);

            // Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        private static PageBannerContentItemApiDataModel BuildValidContentItemApiDataModel()
        {
            var model = new PageBannerContentItemApiDataModel
            {
                Title = "an-article",
                Url = new Uri("https://localhost"),
                Published = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
            };

            return model;
        }

        private PageBannerContentItemModel BuildValidContentItemModel()
        {
            var model = new PageBannerContentItemModel()
            {
                Id = contentIdForUpdate,
                Etag = Guid.NewGuid().ToString(),
                Url = new Uri("https://localhost"),
                LastReviewed = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                LastCached = DateTime.UtcNow,
            };

            return model;
        }
    }
}
