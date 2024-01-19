using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure.Strategy;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBanner;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.Common.SharedContent.Pkg.Netcore.RequestHandler;
using DFC.Common.SharedContent.Pkg.Netcore;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
        private const string RedisCacheConnectionStringAppSettings = "Cms:RedisCacheConnectionString";
        private const string GraphApiUrlAppSettings = "Cms:GraphApiUrl";
        public CustomWebApplicationFactory()
        {
        }


        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder?.ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                services.AddSingleton<IConfiguration>(configuration);
                services.AddStackExchangeRedisCache(options => { options.Configuration = configuration.GetSection(RedisCacheConnectionStringAppSettings).Get<string>(); });

                services.AddHttpClient();
                services.AddSingleton<IGraphQLClient>(s =>
                {
                    var option = new GraphQLHttpClientOptions()
                    {
                        EndPoint = new Uri(configuration.GetSection(GraphApiUrlAppSettings).Get<string>()),

                        HttpMessageHandler = new CmsRequestHandler(s.GetService<IHttpClientFactory>(), s.GetService<IConfiguration>(), s.GetService<IHttpContextAccessor>()),
                    };
                    var client = new GraphQLHttpClient(option, new NewtonsoftJsonSerializer());
                    return client;
                });

                services.AddSingleton<ISharedContentRedisInterfaceStrategy<PageBanner>, PageBannerQueryStrategy>();
                services.AddSingleton<ISharedContentRedisInterfaceStrategy<PageBannerResponse>, PageBannersAllQueryStrategy>();
                services.AddSingleton<ISharedContentRedisInterfaceStrategyFactory, SharedContentRedisStrategyFactory>();
                services.AddScoped<ISharedContentRedisInterface, SharedContentRedis>();
            });
        }

    }
}