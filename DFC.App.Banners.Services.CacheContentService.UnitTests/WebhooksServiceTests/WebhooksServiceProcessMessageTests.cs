using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DFC.App.Banners.Data.Enums;
using DFC.App.Banners.Data.Helpers;
using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using FakeItEasy;
using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service ProcessMessageAsync Unit Tests")]
    public class WebhooksServiceProcessMessageTests : BaseWebhooksServiceTests
    {
        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncNoneOptionReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
            var url = "https://somewhere.com";
            var contentType = CmsContentKeyHelper.PageBannerTag;
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.None, Guid.NewGuid(), ContentIdForCreate, url, contentType);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentThrowsErrorForInvalidUrl()
        {
            // Arrange
            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
            var expectedValidContentItemModel = BuildValidContentItemModel();
            var url = "/somewhere.com";
            var contentType = CmsContentKeyHelper.PageBannerTag;
            var service = BuildWebhooksService();

            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, url, contentType));
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentCreateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
            var expectedValidContentItemModel = BuildValidContentItemModel();
            var url = "https://somewhere.com";
            var contentType = CmsContentKeyHelper.PageBannerTag;
            var service = BuildWebhooksService();

            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, url, contentType);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentUpdateReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
            var expectedValidContentItemModel = BuildValidContentItemModel();
            var url = "https://somewhere.com";
            var contentType = CmsContentKeyHelper.PageBannerTag;
            var service = BuildWebhooksService();


            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForUpdate, url, contentType);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task WebhooksServiceProcessMessageAsyncContentDeleteReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var url = "https://somewhere.com";
            var service = BuildWebhooksService();
            var contentType = CmsContentKeyHelper.PageBannerTag;

            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(true);

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.Delete, Guid.NewGuid(), ContentIdForDelete, url, contentType);

            // Assert
            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResponse, result);
        }
    }
}
