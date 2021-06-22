using System;
using DFC.App.Banners.Data.Models.ContentModels;
using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.WebhooksServiceTests
{
    [Trait("Category", "Webhooks Service TryValidateModel Unit Tests")]
    public class WebhooksServiceTryValidateModelTests : BaseWebhooksServiceTests
    {
        [Fact]
        public void WebhooksServiceTryValidateModelForCreateReturnsSuccess()
        {
            // Arrange
            const bool expectedResponse = true;
            var expectedValidContentItemModel = BuildValidContentItemModel();
            var service = BuildWebhooksService();

            // Act
            var result = service.TryValidateModel(expectedValidContentItemModel);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void WebhooksServiceTryValidateModelForUpdateReturnsFailure()
        {
            // Arrange
            const bool expectedResponse = false;
            var expectedInvalidContentItemModel = new BannerContentItemModel();
            var service = BuildWebhooksService();

            // Act
            var result = service.TryValidateModel(expectedInvalidContentItemModel);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void WebhooksServiceTryValidateModelRaisesExceptionForNullContentItemModel()
        {
            // Arrange
            BannerContentItemModel? nullContentItemModel = null;
            var service = BuildWebhooksService();

            // Act
            var exceptionResult = Assert.Throws<ArgumentNullException>(() => service.TryValidateModel(nullContentItemModel));

            // Assert
            Assert.Equal("Value cannot be null. (Parameter 'sharedContentItemModel')", exceptionResult.Message);
        }
    }
}
