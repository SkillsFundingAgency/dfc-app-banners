using System.Collections.Generic;
using System.Net.Mime;

using AutoMapper;

using DFC.App.Banners.Controllers;
using DFC.Compui.Cosmos.Contracts;

using FakeItEasy;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;

namespace DFC.App.Banners.UnitTests.ControllerTests.BannersControllerTests
{
    public abstract class BaseBannersControllerTests
    {
        protected BaseBannersControllerTests()
        {
            Logger = A.Fake<ILogger<BannersController>>();
            FakeSharedContentRedis = A.Fake<ISharedContentRedisInterface>();
            FakeMapper = A.Fake<IMapper>();
        }

        public static IEnumerable<object[]> HtmlMediaTypes => new List<object[]>
        {
            new string[] { "*/*" },
            new string[] { MediaTypeNames.Text.Html },
        };

        public static IEnumerable<object[]> InvalidMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Text.Plain },
        };

        public static IEnumerable<object[]> JsonMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Application.Json },
        };

        protected ILogger<BannersController> Logger { get; }

        protected ISharedContentRedisInterface FakeSharedContentRedis { get; }


        protected IMapper FakeMapper { get; }

        protected BannersController BuildBannersController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new BannersController(Logger, FakeMapper, FakeSharedContentRedis)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}
