﻿using System.Collections.Generic;
using System.Net.Mime;

using AutoMapper;

using DFC.App.Banners.Controllers;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Compui.Cosmos.Contracts;

using FakeItEasy;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DFC.App.Banners.UnitTests.ControllerTests.BannersControllerTests
{
    public abstract class BaseBannersControllerTests
    {
        private readonly IMemoryCache memoryCache;

        protected BaseBannersControllerTests()
        {
            Logger = A.Fake<ILogger<BannersController>>();
            FakeDocumentService = A.Fake<IDocumentService<PageBannerContentItemModel>>();
            FakeMapper = A.Fake<IMapper>();

            memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        }

        ~BaseBannersControllerTests()
        {
            memoryCache.Dispose();
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

        protected IDocumentService<PageBannerContentItemModel> FakeDocumentService { get; }

        protected IMapper FakeMapper { get; }

        protected BannersController BuildBannersController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new BannersController(Logger, FakeMapper, FakeDocumentService, memoryCache)
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
