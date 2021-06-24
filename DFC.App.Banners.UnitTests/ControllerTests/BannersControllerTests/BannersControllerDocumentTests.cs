using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DFC.App.Banners.Models;
using DFC.App.Banners.UnitTests.ControllerTests.PagesControllerTests;
using DFC.App.Banners.ViewModels;
using DFC.Compui.Cosmos.Models;

using FakeItEasy;

using Microsoft.AspNetCore.Mvc;

using Xunit;

namespace DFC.App.Banners.UnitTests.ControllerTests.BannersControllerTests
{
    public class BannersControllerDocumentTests : BaseBannersControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task BannersControllerDocumentHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var controller = BuildBannersController(mediaTypeName);
           
            // Act
            var result = await controller.GetAsync("/some-location").ConfigureAwait(false);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PageBannerViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }
    }
}
