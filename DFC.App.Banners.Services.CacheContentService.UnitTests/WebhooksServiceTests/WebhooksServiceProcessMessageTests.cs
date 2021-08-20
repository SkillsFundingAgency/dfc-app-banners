using System;
using System.IO;
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
            var expectedResponse = false;
            var url = "https://somewhere.com";
            var eventHandler = AddEventHandler(contentType);
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.None, Guid.NewGuid(), ContentIdForCreate, url, contentType);

            // Assert
            A.CallTo(() => eventHandler!.ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).MustNotHaveHappened();
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
            var expectedResponse = true;
            var url = "https://somewhere.com";
            var eventHandler = AddEventHandler(contentType);
            A.CallTo(() => eventHandler!.ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).Returns(expectedResponse);
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, url, contentType);

            // Assert
            A.CallTo(() => eventHandler!.ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).MustHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Theory]
        [InlineData("Page")]
        [InlineData("SharedContent")]
        [InlineData("PageLocation")]
        public async Task WebhooksServiceProcessMessageAsyncThrowExceptionForNonBannerContentTypes(string contentType)
        {
            // Arrange
            var url = "https://somewhere.com";
            AddEventHandler(contentType);
            var service = BuildWebhooksService();

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForCreate, url, contentType));
        }

        [Theory]
        [InlineData(CmsContentKeyHelper.PageBannerTag)]
        [InlineData(CmsContentKeyHelper.BannerTag)]
        public async Task WebhooksServiceProcessMessageAsyncContentUpdateReturnsSuccess(string contentType)
        {
            // Arrange
            var expectedResponse = true;
            var url = "https://somewhere.com";
            var eventHandler = AddEventHandler(contentType);
            A.CallTo(() => eventHandler!.ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).Returns(expectedResponse);
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForUpdate, url, contentType);

            // Assert
            A.CallTo(() => eventHandler!.ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).MustHaveHappened();

            Assert.Equal(expectedResponse, result);
        }

        [Theory]
        [InlineData(CmsContentKeyHelper.PageBannerTag)]
        [InlineData(CmsContentKeyHelper.BannerTag)]
        public async Task WebhooksServiceProcessMessageAsyncContentCreateThrowsExceptionForInvalidUrl(string contentType)
        {
            // Arrange
            var url = "/pagebanner/290762a7-32e4-46c6-a2bb-6efa7bb6d4c7";
            var eventHandler = AddEventHandler(contentType);
            A.CallTo(() => eventHandler!.ProcessContentAsync(A<Guid>.Ignored, A<Uri>.Ignored)).Returns(true);
            var service = BuildWebhooksService();

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), ContentIdForUpdate, url, contentType));
        }

        [Theory]
        [InlineData(CmsContentKeyHelper.PageBannerTag)]
        [InlineData(CmsContentKeyHelper.BannerTag)]
        public async Task WebhooksServiceProcessMessageAsyncContentDeleteReturnsSuccess(string contentType)
        {
            // Arrange
            var expectedResponse = true;
            var url = "https://somewhere.com";
            var eventHandler = AddEventHandler(contentType);
            A.CallTo(() => eventHandler!.DeleteContentAsync(A<Guid>.Ignored)).Returns(expectedResponse);
            var service = BuildWebhooksService();

            // Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.Delete, Guid.NewGuid(), ContentIdForDelete, url, contentType);

            // Assert
            A.CallTo(() => eventHandler!.DeleteContentAsync(A<Guid>.Ignored)).MustHaveHappened();

            Assert.Equal(expectedResponse, result);
        }
    }
}
