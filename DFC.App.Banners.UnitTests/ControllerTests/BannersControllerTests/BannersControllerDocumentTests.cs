using DFC.App.Banners.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Common;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBanner;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var expectedResults = A.Fake<PageBanner>();
            expectedResults.Banner = new Banner()
            {
                WebPageUrl = "/test",
                WebPageName = "Test",
            };
            expectedResults.GraphSync = new GraphSync()
            {
                NodeId = "<<contentapiprefix>>/banner/test-guidid",
            };
            var controller = BuildBannersController(mediaTypeName);
            _ = A.CallTo(() => FakeSharedContentRedis.GetDataAsync<PageBanner>(A<string>.Ignored, "PUBLISHED", A<double>.Ignored))
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
        public async Task BannersControllerBodyHtmlReturnsNoContent(string mediaTypeName)
        {
            // Arrange
            var controller = BuildBannersController(mediaTypeName);

            // Act
            var result = await controller.BodyAsync("/some-location");

            // Assert
            Assert.IsType<NoContentResult>(result);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task BannersControllerBodyHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var expectedResults = A.Fake<PageBanner>();
            expectedResults.Banner = new Banner()
            {
                WebPageUrl = "/test",
                WebPageName = "Test",
            };
            expectedResults.GraphSync = new GraphSync()
            {
                NodeId = "<<contentapiprefix>>/banner/test-guidid",
            };
            var controller = BuildBannersController(mediaTypeName);
            _ = A.CallTo(() => FakeSharedContentRedis.GetDataAsync<PageBanner>(A<string>.Ignored, "PUBLISHED", A<double>.Ignored))
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
