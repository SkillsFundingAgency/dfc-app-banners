using System.Net.Mime;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.Banners.UnitTests.ControllerTests.RobotControllerTests
{
    [Trait("Category", "Robot Controller Unit Tests")]
    public class RobotControllerViewTests : BaseRobotControllerTests
    {
        [Fact]
        public void RobotControllerRobotViewReturnsSuccess()
        {
            // Arrange
            using var controller = BuildRobotController();

            // Act
            var result = controller.RobotView();

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);

            contentResult.ContentType.Should().Be(MediaTypeNames.Text.Plain);
        }
    }
}
