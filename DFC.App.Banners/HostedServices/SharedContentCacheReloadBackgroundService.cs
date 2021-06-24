using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using DFC.App.Banners.Data.Contracts;
using DFC.Compui.Telemetry.HostedService;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.HostedServices
{
    [ExcludeFromCodeCoverage]
    public class SharedContentCacheReloadBackgroundService : BackgroundService
    {
        private readonly ILogger<SharedContentCacheReloadBackgroundService> logger;
        private readonly CmsApiClientOptions cmsApiClientOptions;
        private readonly ISharedContentCacheReloadService sharedContentCacheReloadService;
        private readonly IHostedServiceTelemetryWrapper hostedServiceTelemetryWrapper;

        public SharedContentCacheReloadBackgroundService(ILogger<SharedContentCacheReloadBackgroundService> logger, CmsApiClientOptions cmsApiClientOptions, ISharedContentCacheReloadService sharedContentCacheReloadService, IHostedServiceTelemetryWrapper hostedServiceTelemetryWrapper)
        {
            this.logger = logger;
            this.cmsApiClientOptions = cmsApiClientOptions;
            this.hostedServiceTelemetryWrapper = hostedServiceTelemetryWrapper;
            this.sharedContentCacheReloadService = sharedContentCacheReloadService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Cache reload started");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Cache reload stopped");

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (cmsApiClientOptions.BaseAddress == null)
                {
                    logger.LogInformation($"CMS Api Client Base Address is null, skipping Cache Reload");
                }

                logger.LogInformation($"Executing Telemetry wrapper with service {nameof(sharedContentCacheReloadService)}");

                var sharedContentCacheReloadServiceTask = hostedServiceTelemetryWrapper.Execute(async () => await sharedContentCacheReloadService.Reload(stoppingToken), nameof(SharedContentCacheReloadBackgroundService));
                await sharedContentCacheReloadServiceTask;

                //Caters for errors in the telemetry wrapper
                if (!sharedContentCacheReloadServiceTask.IsCompletedSuccessfully)
                {
                    logger.LogInformation($"An error occurred in the {nameof(hostedServiceTelemetryWrapper)}");

                    if (sharedContentCacheReloadServiceTask.Exception != null)
                    {
                        logger.LogError(sharedContentCacheReloadServiceTask.Exception.ToString());
                        throw sharedContentCacheReloadServiceTask.Exception;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
