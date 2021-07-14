using DFC.App.Banners.Data.Models.ContentModels;
using FakeItEasy;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.DocumentServiceTests
{
    [Trait("Category", "Banner Document Service Unit Tests")]
    public class BannerDocumentServiceTests : BaseBannerDocumentServiceTests
    {
        [Theory]
        [InlineData(true)]
        public async Task BannerDocumentServiceDeleteAsyncDeleteByGuidId(bool documentServiceResponse)
        {
            // Arrage
            A.CallTo(() => FakeDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(documentServiceResponse);
            var service = BuildBannerDocumentService();

            // Act
            var result = await service.DeleteAsync(ContentIdForDelete);

            //Assert
            A.CallTo(() => FakeDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(result, documentServiceResponse);
        }

        [Fact]
        public async Task BannerDocumentServiceGetByIdAsyncReturnsPageBannerContentItemModels()
        {
            // Arrage
            var expectedPageBannerContentItemModel = BuildValidPageBannerContentItemModel();
            A.CallTo(() => FakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedPageBannerContentItemModel);
            var service = BuildBannerDocumentService();

            // Act
            var result = await service.GetByIdAsync(ContentIdForUpdate, "/");

            //Assert
            A.CallTo(() => FakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(result?.Id, expectedPageBannerContentItemModel.Id);
        }

        [Fact]
        public async Task BannerDocumentServiceUpsertAsyncReturnsSuccess()
        {
            // Arrage
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            A.CallTo(() => FakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).Returns(expectedResult);
            var service = BuildBannerDocumentService();

            // Act
            var result = await service.UpsertAsync(BuildValidPageBannerContentItemModel());

            //Assert
            A.CallTo(() => FakeDocumentService.UpsertAsync(A<PageBannerContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public async Task BannerDocumentServiceGetPagebannerUrlsAsyncReturnsPageBannerUrls()
        {
            // Arrage
            var expectedResult = new List<string>
            {
                "http://localhost:1234/api/execute/pagebanner/3a3fff39-0efb-450e-8893-80370656695b",
                "http://localhost:1234/api/execute/pagebanner/4a3fff39-0efb-450e-8893-80370656696z",
            };

            var response = new FeedResponse<PageBannerContentItemModel>(BuildValidPageBannerContentItemModels(expectedResult));

            A.CallTo(() => FakeDocumentQuery.HasMoreResults).Returns(true).Once().Then.Returns(false);

            A.CallTo(() => FakeDocumentQuery.ExecuteNextAsync<PageBannerContentItemModel>(A<CancellationToken>.Ignored)).Returns(response);

            A.CallTo(() => FakeDocumentClient.CreateDocumentQuery<IEnumerable<string>>(A<Uri>.Ignored, A<SqlQuerySpec>.Ignored, A<FeedOptions>.Ignored)).Returns(FakeDocumentQuery);

            var service = BuildBannerDocumentService();

            // Act
            IEnumerable<Uri>? result = await service.GetPagebannerUrlsAsync(ContentIdForUpdate.ToString(), null);

            //Assert
            result.Should().NotBeEmpty()
                  .And.HaveCount(2)
                  .And.ContainInOrder(expectedResult)
                  .And.ContainItemsAssignableTo<Uri>();
        }

        [Fact]
        public async Task BannerDocumentServiceGetPagebannerUrlsAsyncReturnsEmptyList()
        {
            // Arrage
            var expectedResult = new List<string>();

            var response = new FeedResponse<PageBannerContentItemModel>(new List<PageBannerContentItemModel>());

            A.CallTo(() => FakeDocumentQuery.HasMoreResults).Returns(true).Once().Then.Returns(false);

            A.CallTo(() => FakeDocumentQuery.ExecuteNextAsync<PageBannerContentItemModel>(A<CancellationToken>.Ignored)).Returns(response);

            A.CallTo(() => FakeDocumentClient.CreateDocumentQuery<IEnumerable<string>>(A<Uri>.Ignored, A<SqlQuerySpec>.Ignored, A<FeedOptions>.Ignored)).Returns(FakeDocumentQuery);

            A.CallTo(() => FakeDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(BuildValidPageBannerContentItemModel());

            var service = BuildBannerDocumentService();

            // Act
            IEnumerable<Uri>? result = await service.GetPagebannerUrlsAsync(ContentIdForUpdate.ToString(), null);

            //Assert
            result.Should().BeEmpty();
        }
    }
}