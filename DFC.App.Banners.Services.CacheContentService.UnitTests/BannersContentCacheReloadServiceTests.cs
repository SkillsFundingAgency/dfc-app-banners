using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DFC.App.Banners.Data.Helpers;
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

        [Fact]
        public async Task SharedContentCacheReloadServiceReloadAllCancellationRequestedCancels()
        {
            //Arrange
            var cancellationToken = new CancellationToken(true);
            var contentCacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeCmsApiService);

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
            var contentCacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeCmsApiService);

            //Act
            await contentCacheReloadService.Reload(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustHaveHappened(SharedContentKeyHelper.GetSharedContentKeys().Count(), Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappened(SharedContentKeyHelper.GetSharedContentKeys().Count(), Times.Exactly);
        }

        [Fact]
        public async Task SharedContentCacheReloadServiceReloadSharedContentSuccessful()
        {
            //Arrange
            var dummyContentItem = A.Dummy<PageBannerContentItemApiDataModel>();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(dummyContentItem);
            var sharedContentCacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeCmsApiService);

            //Act
            await sharedContentCacheReloadService.ReloadContent(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustHaveHappened(SharedContentKeyHelper.GetSharedContentKeys().Count(), Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappened(SharedContentKeyHelper.GetSharedContentKeys().Count(), Times.Exactly);
        }

        [Fact]
        public async Task SharedContentCacheReloadServiceReloadSharedContentNullApiResponse()
        {
            //Arrange
            PageBannerContentItemApiDataModel? nullContentItem = null;

            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(nullContentItem);
            var sharedContentCacheReloadService = new BannersCacheReloadService(A.Fake<ILogger<BannersCacheReloadService>>(), fakeMapper, fakeDocumentService, fakeCmsApiService);

            //Act
            await sharedContentCacheReloadService.ReloadContent(CancellationToken.None);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustHaveHappened(SharedContentKeyHelper.GetSharedContentKeys().Count(), Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
        }
    }
}
