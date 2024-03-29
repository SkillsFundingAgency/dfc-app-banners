﻿using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.HostedServices;
using DFC.App.Banners.Services.CacheContentService;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DFC.App.Banners
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string CosmosDbContentBannersConfigAppSettings = "Configuration:CosmosDbConnections:ContentBanners";

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
            var cosmosDbConnectionSharedContent = configuration.GetSection(CosmosDbContentBannersConfigAppSettings).Get<CosmosDbConnection>();
            var cosmosRetryOptions = new Microsoft.Azure.Documents.Client.RetryOptions
            {
                MaxRetryAttemptsOnThrottledRequests = 20,
                MaxRetryWaitTimeInSeconds = 60,
            };
            services.AddDocumentServices<PageBannerContentItemModel>(cosmosDbConnectionSharedContent, env.IsDevelopment(), cosmosRetryOptions);

            services.AddMemoryCache();
            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();
            services.AddTransient<IBannersCacheReloadService, BannersCacheReloadService>();
            services.AddTransient<IWebhooksService, WebhooksService>();
            services.AddTransient<IBannerDocumentService, BannerDocumentService>();
            services.AddTransient<IEventHandler, BannerEventHandler>();
            services.AddTransient<IEventHandler, PageBannerEventHandler>();

            services.AddAutoMapper(typeof(Startup).Assembly);
            CmsApiClientOptions cmsApiClientOptions = configuration.GetSection(nameof(CmsApiClientOptions)).Get<CmsApiClientOptions>();
            services.AddSingleton(cmsApiClientOptions ?? new CmsApiClientOptions());
            services.AddHostedServiceTelemetryWrapper();
            services.AddHostedService<CacheReloadBackgroundService>();
            services.AddSubscriptionBackgroundService(configuration);

            var policyRegistry = services.AddPolicyRegistry();

            services.AddApiServices(configuration, policyRegistry);

            services.AddMvc(config =>
                {
                    config.RespectBrowserAcceptHeader = true;
                    config.ReturnHttpNotAcceptable = true;
                })
                .AddNewtonsoftJson();
        }
    }
}