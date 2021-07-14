using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DFC.App.Banners.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.Banners.UnitTests.ControllerTests.HealthControllerTests
{
    [Trait("Category", "Health Controller Unit Tests")]
    public class HealthControllerViewTests : BaseHealthControllerTests
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task HealthControllerViewHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            bool expectedResult = true;
            using var controller = BuildHealthController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.HealthView();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<HealthViewModel>(viewResult.ViewData.Model);
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task HealthControllerViewJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            bool expectedResult = true;
            using var controller = BuildHealthController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.HealthView();

            // Assert
            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<IList<HealthItemViewModel>>(jsonResult.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task HealthControllerHealthViewReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            bool expectedResult = true;
            using var controller = BuildHealthController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.HealthView();

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);
        }
    }
}
