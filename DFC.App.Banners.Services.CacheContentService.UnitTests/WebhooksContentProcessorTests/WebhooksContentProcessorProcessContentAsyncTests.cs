//using System;
//using System.Net;
//using System.Threading.Tasks;
//using DFC.App.Banners.Data.Models.CmsApiModels;
//using DFC.App.Banners.Data.Models.ContentModels;
//using FakeItEasy;
//using Xunit;

//namespace DFC.App.Banners.Services.CacheContentService.UnitTests.WebhooksContentProcessorTests
//{
//    [Trait("Category", "Webhooks Content Processor ProcessContentAsync Unit Tests")]
//    public class WebhooksContentProcessorProcessContentAsyncTests : BaseWebhooksContentProcessorTests
//    {
//        [Fact]
//        public async Task WebhooksContentProcessorProcessContentAsyncForCreateReturnsSuccess()
//        {
//            // Arrange
//            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
//            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
//            var expectedValidContentItemModel = BuildValidContentItemModel();
//            var url = new Uri("https://somewhere.com");
//            var service = BuildWebhooksContentProcessor();

//            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
//            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);
//            A.CallTo(() => FakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentItemModel);
//            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(HttpStatusCode.Created);

//            // Act
//            var result = await service.ProcessContentAsync(url);

//            // Assert
//            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
//            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
//            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

//            Assert.Equal(expectedResponse, result);
//        }

//        [Fact]
//        public async Task WebhooksContentProcessorProcessContentAsyncForUpdateReturnsSuccess()
//        {
//            // Arrange
//            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
//            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
//            var expectedValidContentItemModel = BuildValidContentItemModel();
//            var url = new Uri("https://somewhere.com");
//            var service = BuildWebhooksContentProcessor();

//            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
//            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);
//            A.CallTo(() => FakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedValidContentItemModel);
//            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(HttpStatusCode.OK);

//            // Act
//            var result = await service.ProcessContentAsync(url);

//            // Assert
//            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
//            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
//            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

//            Assert.Equal(expectedResponse, result);
//        }

//        [Fact]
//        public async Task WebhooksContentProcessorProcessContentAsyncForUpdateReturnsNoContent()
//        {
//            // Arrange
//            const HttpStatusCode expectedResponse = HttpStatusCode.NoContent;
//            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
//            PageBannerContentItemModel? expectedValidContentItemModel = default;
//            var url = new Uri("https://somewhere.com");
//            var service = BuildWebhooksContentProcessor();

//            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
//            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel?>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);

//            // Act
//            var result = await service.ProcessContentAsync(url);

//            // Assert
//            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
//            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
//            A.CallTo(() => FakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
//            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

//            Assert.Equal(expectedResponse, result);
//        }

//        [Fact]
//        public async Task WebhooksContentProcessorProcessContentAsyncForUpdateReturnsBadRequest()
//        {
//            // Arrange
//            const HttpStatusCode expectedResponse = HttpStatusCode.BadRequest;
//            var expectedValidContentItemApiDataModel = BuildValidContentItemApiDataModel();
//            var expectedValidContentItemModel = new PageBannerContentItemModel();
//            var url = new Uri("https://somewhere.com");
//            var service = BuildWebhooksContentProcessor();

//            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidContentItemApiDataModel);
//            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).Returns(expectedValidContentItemModel);

//            // Act
//            var result = await service.ProcessContentAsync(url);

//            // Assert
//            A.CallTo(() => FakeCmsApiService.GetItemAsync<PageBannerContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
//            A.CallTo(() => FakeMapper.Map<PageBannerContentItemModel>(A<PageBannerContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
//            A.CallTo(() => FakeBannerDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
//            A.CallTo(() => FakeBannerDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustNotHaveHappened();
//            A.CallTo(() => FakeBannerDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

//            Assert.Equal(expectedResponse, result);
//        }
//    }
//}
