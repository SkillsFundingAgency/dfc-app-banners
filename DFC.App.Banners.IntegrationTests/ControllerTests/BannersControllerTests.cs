using System.Threading.Tasks;

using Xunit;

namespace DFC.App.Banners.IntegrationTests.ControllerTests
{
    public class BannersControllerTests
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public BannersControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestAsync()
        {
            await Task.CompletedTask;
        }
    }
}
