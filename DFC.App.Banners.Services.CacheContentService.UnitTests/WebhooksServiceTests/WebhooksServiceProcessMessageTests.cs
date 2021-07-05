using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DFC.App.Banners.Data.Enums;
using DFC.App.Banners.Data.Helpers;
using FakeItEasy;
using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service ProcessMessageAsync Unit Tests")]
    public class WebhooksServiceProcessMessageTests : BaseWebhooksServiceTests
    {
        [Theory]
        [InlineData(CmsContentKeyHelper.PageBannerTag)]
        [InlineData(CmsContentKeyHelper.BannerTag)]
        public async Task WebhooksServiceProcessMessageAsyncNoneOptionReturnsSuccess(string contentType)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
            var url = "https://somewhere.com";
            var eventHandler = AddEventHandler(contentType);
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.None, Guid.NewGuid(), ContentIdForCreate, url, contentType);

            // Assert
            A.CallTo(() => eventHandler.ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
            Assert.Equal(expectedResponse, result);
        }

        [Theory]
        [InlineData(CmsContentKeyHelper.PageBannerTag)]
        [InlineData(CmsContentKeyHelper.BannerTag)]
        public async Task WebhooksServiceProcessMessageAsyncContentThrowsErrorForInvalidUrl(string contentType)
        {
            // Arrange
            var url = "/somewhere.com";
            var service = BuildWebhooksService();

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, url, contentType));
        }

        [Theory]
        [InlineData(CmsContentKeyHelper.PageBannerTag)]
        [InlineData(CmsContentKeyHelper.BannerTag)]
        public async Task WebhooksServiceProcessMessageAsyncContentCreateReturnsSuccess(string contentType)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            var url = "https://somewhere.com";
            A.CallTo(() => AddEventHandler(contentType).ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).Returns(HttpStatusCode.Created);
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, url, contentType);

            // Assert
            A.CallTo(() => AddEventHandler(contentType).ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Theory]
        [InlineData(CmsContentKeyHelper.PageBannerTag)]
        [InlineData(CmsContentKeyHelper.BannerTag)]
        public async Task WebhooksServiceProcessMessageAsyncContentUpdateReturnsSuccess(string contentType)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var url = "https://somewhere.com";
            A.CallTo(() => AddEventHandler(contentType).ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).Returns(HttpStatusCode.OK);
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForUpdate, url, contentType);

            // Assert
            A.CallTo(() => AddEventHandler(contentType).ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Theory]
        [InlineData(CmsContentKeyHelper.PageBannerTag)]
        [InlineData(CmsContentKeyHelper.BannerTag)]
        public async Task WebhooksServiceProcessMessageAsyncContentDeleteReturnsSuccess(string contentType)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var url = "https://somewhere.com";
            A.CallTo(() => AddEventHandler(contentType).DeleteContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).Returns(HttpStatusCode.OK);
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.Delete, Guid.NewGuid(), ContentIdForDelete, url, contentType);

            // Assert
            A.CallTo(() => AddEventHandler(contentType).ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}
