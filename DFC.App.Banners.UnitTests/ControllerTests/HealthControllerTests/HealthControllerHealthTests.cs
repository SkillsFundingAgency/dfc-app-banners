using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using DFC.App.Banners.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.Banners.UnitTests.ControllerTests.HealthControllerTests
{
    [Trait("Category", "Health Controller Unit Tests")]
    public class HealthControllerHealthTests : BaseHealthControllerTests
    {
        [Fact]
        public async Task HealthControllerHealthReturnsSuccessWhenHealthy()
        {
            // Arrange
            bool expectedResult = true;
            using var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => FakeDocumentService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.Health();

            // Assert
            A.CallTo(() => FakeDocumentService.PingAsync()).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var models = Assert.IsAssignableFrom<List<HealthItemViewModel>>(jsonResult.Value);

            models.Count.Should().BeGreaterThan(0);
            models.First().Service.Should().NotBeNullOrWhiteSpace();
            models.First().Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task HealthControllerHealthReturnsServiceUnavailableWhenUnhealthy()
        {
            // Arrange
            bool expectedResult = false;
            using var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => FakeDocumentService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.Health();

            // Assert
            A.CallTo(() => FakeDocumentService.PingAsync()).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.ServiceUnavailable, statusResult.StatusCode);
        }
    }
}
