//using System;
//using System.Threading.Tasks;

//using DFC.App.Banners.Data.Contracts;
//using DFC.App.Banners.HostedServices;
//using DFC.Compui.Telemetry.HostedService;
//using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;

//using FakeItEasy;

//using Microsoft.Extensions.Logging;

//using Xunit;

//namespace DFC.App.Banners.UnitTests
//{
//    public class CacheReloadBackgroundServiceTests
//    {
//        private readonly IBannersCacheReloadService cacheReloadService = A.Fake<IBannersCacheReloadService>();
//        private readonly ILogger<CacheReloadBackgroundService> logger = A.Fake<ILogger<CacheReloadBackgroundService>>();
//        private readonly IHostedServiceTelemetryWrapper wrapper = A.Fake<IHostedServiceTelemetryWrapper>();

//        [Fact]
//        public async Task CacheReloadBackgroundServiceStartsAsyncCompletesSuccessfully()
//        {
//            // Arrange
//            A.CallTo(() => wrapper.Execute(A<Func<Task>>.Ignored, A<string>.Ignored)).Returns(Task.CompletedTask);
//            var serviceToTest = new CacheReloadBackgroundService(logger, new CmsApiClientOptions { BaseAddress = new Uri("http://somewhere.com") }, cacheReloadService, wrapper);

//            // Act
//            await serviceToTest.StartAsync(default);

//            // Assert
//            A.CallTo(() => wrapper.Execute(A<Func<Task>>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
//            serviceToTest.Dispose();
//        }

//        [Fact]
//        public async Task CacheReloadBackgroundServiceStartsAsyncThrowsException()
//        {
//            // Arrange
//            const string Message = "An Exception";
//            A.CallTo(() => wrapper.Execute(A<Func<Task>>.Ignored, A<string>.Ignored)).Returns(Task.FromException(new Exception(Message)));
//            var serviceToTest = new CacheReloadBackgroundService(logger, new CmsApiClientOptions { BaseAddress = new Uri("http://somewhere.com") }, cacheReloadService, wrapper);

//            // Act
//            // Assert
//            var ex = await Assert.ThrowsAsync<AggregateException>(async () => await serviceToTest.StartAsync(default));
//            Assert.NotNull(ex.InnerException);
//            Assert.Equal(ex.InnerException?.Message, Message);
//            serviceToTest.Dispose();
//        }
//    }
//}
