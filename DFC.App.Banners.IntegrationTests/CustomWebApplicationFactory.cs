using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.IntegrationTests.Extensions;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DFC.App.Banners.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        public CustomWebApplicationFactory()
        {
            this.MockCosmosRepo = A.Fake<ICosmosRepository<PageBannerContentItemModel>>();
        }

        internal ICosmosRepository<PageBannerContentItemModel> MockCosmosRepo { get; set; }

        public HttpClient CreateClientWithWebHostBuilder()
        {
            return this.WithWebHostBuilder(builder =>
            {
                builder.RegisterServices(this.MockCosmosRepo);
            }).CreateClient();
        }

        internal static IEnumerable<PageBannerContentItemModel> GetContentPageModels()
        {
            return new List<PageBannerContentItemModel>
            {
                new PageBannerContentItemModel
                {
                    Id = Guid.NewGuid(),
                    Url = new Uri("http://www.test.com"),
                },
                new PageBannerContentItemModel
                {
                    Id = Guid.NewGuid(),
                    Url = new Uri("http://www.test.com"),
                },
            };
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder?.ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                services.AddSingleton<IConfiguration>(configuration);
            });
        }
    }
}