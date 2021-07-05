using System;
using DFC.App.Banners.Data.Models.ContentModels;
using Xunit;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.WebhooksContentProcessorTests
{
    [Trait("Category", "Webhooks Content Processor TryValidateModel Unit Tests")]
    public class WebhooksContentProcessorTryValidateModelTests : BaseWebhooksContentProcessorTests
    {
        [Fact]
        public void WebhooksContentProcessorTryValidateModelForCreateReturnsSuccess()
        {
            // Arrange
            const bool expectedResponse = true;
            var expectedValidContentItemModel = BuildValidContentItemModel();
            var service = BuildWebhooksContentProcessor();

            // Act
            var result = service.TryValidateModel(expectedValidContentItemModel);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void WebhooksContentProcessorTryValidateModelForUpdateReturnsFailure()
        {
            // Arrange
            const bool expectedResponse = false;
            var expectedInvalidContentItemModel = new PageBannerContentItemModel();
            var service = BuildWebhooksContentProcessor();

            // Act
            var result = service.TryValidateModel(expectedInvalidContentItemModel);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public void WebhooksContentProcessorTryValidateModelRaisesExceptionForNullContentItemModel()
        {
            // Arrange
            PageBannerContentItemModel? nullContentItemModel = null;
            var service = BuildWebhooksContentProcessor();

            // Act
            var exceptionResult = Assert.Throws<ArgumentNullException>(() => service.TryValidateModel(nullContentItemModel));

            // Assert
            Assert.Equal("Value cannot be null. (Parameter 'pageBannerContentItemModel')", exceptionResult.Message);
        }
    }
}
