using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using AutoMapper;
using DFC.Common.SharedContent.Pkg.Netcore;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure.Strategy;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.RequestHandler;
using DFC.Compui.Telemetry;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBanner;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;

namespace DFC.App.Banners
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string RedisCacheConnectionStringAppSettings = "Cms:RedisCacheConnectionString";
        private const string GraphApiUrlAppSettings = "Cms:GraphApiUrl";

        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                // add the default route
                endpoints.MapControllerRoute("default", "{controller=Health}/{action=Ping}");
            });
            mapper?.ConfigurationProvider.AssertConfigurationIsValid();
        }

        public void ConfigureServices(IServiceCollection services)
        {

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

            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();

            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddHostedServiceTelemetryWrapper();


            services.AddMvc(config =>
                {
                    config.RespectBrowserAcceptHeader = true;
                    config.ReturnHttpNotAcceptable = true;
                })
                .AddNewtonsoftJson();
        }
    }
}