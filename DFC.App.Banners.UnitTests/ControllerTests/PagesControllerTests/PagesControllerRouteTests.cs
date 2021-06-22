using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using DFC.App.Banners.Controllers;
using DFC.App.Banners.Data.Models.ContentModels;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace DFC.App.Banners.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerRouteTests : BasePagesControllerTests
    {
        public static IEnumerable<object[]> PagesRouteDataOk => new List<object[]>
        {
            new object[] { "/", Guid.Empty, nameof(PagesController.Index) },
            new object[] { "/pages", Guid.Empty, nameof(PagesController.Index) },
            new object[] { "/pages/{documentId}", Guid.NewGuid(), nameof(PagesController.Document) },
        };

        [Theory]
        [MemberData(nameof(PagesRouteDataOk))]
        public async Task PagesControllerCallsContentPageServiceUsingPagesRouteForOkResult(string route, Guid documentId, string actionMethod)
        {
            // Arrange
            using var controller = BuildController(route);
            var expectedResult = new SharedContentItemModel() { Content = "<h1>A document</h1>" };
            var expectedResults = new List<SharedContentItemModel> { expectedResult };

            A.CallTo(() => FakeSharedContentItemDocumentService.GetAllAsync(A<string>.Ignored)).Returns(expectedResults);
            A.CallTo(() => FakeSharedContentItemDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await RunControllerAction(controller, documentId, actionMethod);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            A.CallTo(() => FakeSharedContentItemDocumentService.GetAllAsync(A<string>.Ignored)).MustHaveHappenedOnceOrLess();
            A.CallTo(() => FakeSharedContentItemDocumentService.GetByIdAsync(A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceOrLess();
        }

        private static async Task<IActionResult> RunControllerAction(PagesController controller, Guid documentId, string actionName)
        {
            return actionName switch
            {
                nameof(PagesController.Document) => await controller.Document(documentId),
                _ => await controller.Index(),
            };
        }

        private PagesController BuildController(string route)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = route;
            httpContext.Request.Headers[HeaderNames.Accept] = MediaTypeNames.Application.Json;

            return new PagesController(Logger, FakeMapper, FakeSharedContentItemDocumentService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                },
            };
        }
    }
}