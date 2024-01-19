using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBanner;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Banners.IntegrationTests.ControllerTests
{
    [Trait("Category", "Banners Controller Integration")]
    public class BannersControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;

        public BannersControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        public static IEnumerable<object[]> BannersNoContentRouteData => new List<object[]>
        {
            new object[] { "/banners/document/careers-advice-test" },
            new object[] { "/banners/document/reers-advice/career-choices-at-16-test" },
            new object[] { "/banners/document/action-plans-test" },
            new object[] { "/banners/document/contact-us-test" },
            new object[] { "/banners/body/careers-advice-test" },
            new object[] { "/banners/body/reers-advice/career-choices-at-16-test" },
            new object[] { "/banners/body/action-plans-test" },
            new object[] { "/banners/body/contact-us-test" },
        };

        [Fact]
        public async Task GetBannersReturnsSuccess()
        {
            // Arrange
            var uri = new Uri("/", UriKind.Relative);
            var client = this.factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            // Act
            var response = await client.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{MediaTypeNames.Text.Html}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [MemberData(nameof(BannersNoContentRouteData))]
        public async Task GetBannersDocumnetReturnsSuccessAndNoContent(string path)
        {
            // Arrange
            var uri = new Uri(path, UriKind.Relative);
            var client = this.factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            // Act
            var response = await client.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Theory]
        [MemberData(nameof(BannersNoContentRouteData))]
        public async Task GetBannersBodyReturnsSuccessAndNoContent(string path)
        {
            // Arrange
            var uri = new Uri(path, UriKind.Relative);
            var pathtest = "actionplans-test";
            this.factory.MockSharedContentRedis.Setup(
                x => x.GetDataAsync<PageBanner>(
                    It.IsAny<string>()))
            .ReturnsAsync(new PageBanner());
            var test = await this.factory.MockSharedContentRedis.Object.GetDataAsync<PageBanner>($"pagebanner/https://nationalcareers.service.gov.uk/{pathtest}");
            var client = this.factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            // Act
            var response = await client.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}