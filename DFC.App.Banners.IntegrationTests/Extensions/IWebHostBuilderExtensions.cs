using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.IntegrationTests.Fakes;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DFC.App.Banners.IntegrationTests.Extensions
{
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder RegisterServices(
            this IWebHostBuilder webHostBuilder, ICosmosRepository<PageBannerContentItemModel> cosmosRepository)
        {
            return webHostBuilder.ConfigureTestServices(services =>
            {
                services.AddTransient(sp => cosmosRepository);
                services.AddTransient<IWebhooksService, FakeWebhooksService>();
            });
        }
    }
}
