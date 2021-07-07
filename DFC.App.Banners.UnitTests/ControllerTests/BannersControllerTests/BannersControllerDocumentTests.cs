using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.ViewModels;

using FakeItEasy;

using Microsoft.AspNetCore.Mvc;

using Xunit;

namespace DFC.App.Banners.UnitTests.ControllerTests.BannersControllerTests
{
    public class BannersControllerDocumentTests : BaseBannersControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task BannersControllerDocumentHtmlReturnsNoContent(string mediaTypeName)
        {
            // Arrange
            var controller = BuildBannersController(mediaTypeName);

            // Act
            var result = await controller.DocumentAsync("/some-location");

            // Assert
            Assert.IsType<NoContentResult>(result);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task BannersControllerDocumentHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var expectedResults = A.CollectionOfDummy<PageBannerContentItemModel>(1);
            var controller = BuildBannersController(mediaTypeName);
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<PageBannerContentItemModel, bool>>>.Ignored))
                .Returns(expectedResults);

            // Act
            var result = await controller.DocumentAsync("/some-location");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<PageBannerViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task BannersControllerBodyHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var expectedResults = A.CollectionOfDummy<PageBannerContentItemModel>(1);
            expectedResults[0].Banners = new List<BannerContentItemModel>(A.CollectionOfDummy<BannerContentItemModel>(1));
            var controller = BuildBannersController(mediaTypeName);
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<PageBannerContentItemModel, bool>>>.Ignored))
                .Returns(expectedResults);

            // Act
            var result = await controller.BodyAsync("/some-location");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<List<BannerViewModel>>(viewResult.ViewData.Model);

            controller.Dispose();
        }
    }
}
