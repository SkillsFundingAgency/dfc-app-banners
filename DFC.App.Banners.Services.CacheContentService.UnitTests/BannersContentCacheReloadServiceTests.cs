using System;
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
        private readonly IContentTypeMappingService fakeContentTypeMappingService = A.Fake<IContentTypeMappingService>();

        // TODO: fake getsummary to return list of pagebanners, and Assert GetItemAsync to have been called appropriate number of times.
        [Fact]
        public async Task SharedContentCacheReloadServiceReloadAllCancellationRequestedCancels()
        {
            //Arrange
            var cancellationToken = new CancellationToken(true);
            var contentCacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeCmsApiService);

            //Act
            await contentCacheReloadService.Reload(cancellationToken);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SharedContentCacheReloadServiceReloadAllReloadsItems()
        {
            //Arrange
            var dummyContentItem = A.Dummy<PageBannerContentItemApiDataModel>();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(dummyContentItem);
            var contentCacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeCmsApiService);

            //Act
            await contentCacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustHaveHappened(0, Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task SharedContentCacheReloadServiceReloadSharedContentSuccessful()
        {
            //Arrange
            var dummyContentItem = A.Dummy<PageBannerContentItemApiDataModel>();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(dummyContentItem);
            var sharedContentCacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeCmsApiService);

            //Act
            await sharedContentCacheReloadService.ReloadContent(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustHaveHappened(0, Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public async Task SharedContentCacheReloadServiceReloadSharedContentNullApiResponse()
        {
            //Arrange
            PageBannerContentItemApiDataModel? nullContentItem = null;

            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(nullContentItem);
            var sharedContentCacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeContentTypeMappingService, fakeCmsApiService);

            //Act
            await sharedContentCacheReloadService.ReloadContent(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustHaveHappened(0, Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }
    }
}
