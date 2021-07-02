using System.Threading.Tasks;

using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.ViewModels;

using FakeItEasy;

using Microsoft.AspNetCore.Mvc;

using Xunit;

namespace DFC.App.Banners.UnitTests.ControllerTests.BannersControllerTests
{
    public class BannersControllerIndexTests : BaseBannersControllerTests
    {

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerIndexHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            int resultsCount = 2;
            var expectedResults = A.CollectionOfDummy<PageBannerContentItemModel>(resultsCount);
            var controller = BuildBannersController(mediaTypeName);
            A.CallTo(() => FakeDocumentService.GetAllAsync(A<string>.Ignored))
                .Returns(expectedResults);

            // Act
            var result = await controller.IndexAsync();

            // Assert
            A.CallTo(() => FakeDocumentService.GetAllAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            A.Equals(resultsCount, model.Documents!.Count);

            controller.Dispose();
        }
    }
}
