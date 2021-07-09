//using System;
//using System.Net;
//using System.Threading.Tasks;
//using FakeItEasy;
//using Xunit;

//namespace DFC.App.Banners.Services.CacheContentService.UnitTests.WebhooksContentProcessorTests
//{
//    [Trait("Category", "Webhooks Content Processor DeleteContentAsync Unit Tests")]
//    public class WebhooksContentProcessorDeleteContentAsyncTests : BaseWebhooksContentProcessorTests
//    {
//        [Fact]
//        public async Task WebhooksContentProcessorDeleteContentAsyncForDeleteReturnsSuccess()
//        {
//            // Arrange
//            const bool expectedResponse = true;
//            const HttpStatusCode expectedResult = HttpStatusCode.OK;
//            var service = BuildWebhooksContentProcessor();

//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResponse);

//            // Act
//            var result = await service.DeleteContentAsync(ContentIdForDelete);

//            // Assert
//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

//            Assert.Equal(expectedResult, result);
//        }

//        [Fact]
//        public async Task WebhooksContentProcessorDeleteContentAsyncForDeleteReturnsNoContent()
//        {
//            // Arrange
//            const bool expectedResponse = false;
//            const HttpStatusCode expectedResult = HttpStatusCode.NoContent;
//            var service = BuildWebhooksContentProcessor();

//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(expectedResponse);

//            // Act
//            var result = await service.DeleteContentAsync(ContentIdForDelete);

//            // Assert
//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

//            Assert.Equal(expectedResult, result);
//        }
//    }
//}
