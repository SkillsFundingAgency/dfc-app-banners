using DFC.App.Banners.Data.Helpers;
using DFC.App.Banners.Models;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.Banners.IntegrationTests.ControllerTests
{
    [Trait("Category", "Webhooks Controller Integration")]
    public class WebhooksControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private const string WebhookApiUrl = "/api/webhook/ReceiveEvents";

        private readonly CustomWebApplicationFactory<Startup> factory;

        public WebhooksControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task WebhooksControllerRouteTestsSubscriptionValidationReturnsSuccess()
        {
            // Arrange
            string expectedValidationCode = Guid.NewGuid().ToString();
            var eventId = Guid.NewGuid().ToString();
            var eventGridEvents = BuildValidEventGridEvent(eventId, EventTypes.EventGridSubscriptionValidationEvent, new SubscriptionValidationEventData(expectedValidationCode, "https://somewhere.com"));
            var uri = new Uri(WebhookApiUrl, UriKind.Relative);
            var client = this.factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            // Act
            var response = await client.PostAsJsonAsync(uri, eventGridEvents);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("published", CmsContentKeyHelper.BannerTag)]
        [InlineData("deleted", CmsContentKeyHelper.BannerTag)]
        [InlineData("draft", CmsContentKeyHelper.BannerTag)]
        [InlineData("draft-discarded", CmsContentKeyHelper.BannerTag)]
        [InlineData("unpublished", CmsContentKeyHelper.BannerTag)]
        [InlineData("published", CmsContentKeyHelper.PageBannerTag)]
        [InlineData("deleted", CmsContentKeyHelper.PageBannerTag)]
        [InlineData("draft", CmsContentKeyHelper.PageBannerTag)]
        [InlineData("draft-discarded", CmsContentKeyHelper.PageBannerTag)]
        [InlineData("unpublished", CmsContentKeyHelper.PageBannerTag)]
        public async Task WebhooksControllerRouteTestsReturnsSuccess(string eventTtype, string contentType)
        {
            // Arrange
            var contentItemId = "9e0e237d-bb0e-4c46-9534-94ce2b55f98d";
            var eventId = Guid.NewGuid().ToString();
            EventGridEventData eventgridEventData = BuildEventGridEventData(contentType, contentItemId);

            var eventGridEvents = BuildValidEventGridEvent(eventId, eventTtype, eventgridEventData);

            var uri = new Uri(WebhookApiUrl, UriKind.Relative);
            var client = this.factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            // Act
            var response = await client.PostAsJsonAsync(uri, eventGridEvents);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("published", CmsContentKeyHelper.BannerTag)]
        [InlineData("published", CmsContentKeyHelper.PageBannerTag)]
        public async Task WebhooksControllerRouteTestsThrowExceptionForInvalidEventId(string eventTtype, string contentType)
        {
            // Arrange
            var contentItemId = "9e0e237d-bb0e-4c46-9534-94ce2b55f98d";
            EventGridEventData eventgridEventData = BuildEventGridEventData(contentType, contentItemId);

            var eventGridEvents = BuildValidEventGridEvent(null, eventTtype, eventgridEventData);

            var uri = new Uri(WebhookApiUrl, UriKind.Relative);
            var client = this.factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            // Act
            var httpResponse = await client.PostAsJsonAsync(uri, eventGridEvents);

            Assert.Equal(httpResponse.StatusCode.ToString(), System.Net.HttpStatusCode.InternalServerError.ToString());
        }

        [Theory]
        [InlineData("published", CmsContentKeyHelper.BannerTag)]
        [InlineData("published", CmsContentKeyHelper.PageBannerTag)]
        public async Task WebhooksControllerRouteTestsThrowExceptionForInvalidContentItemId(string eventTtype, string contentType)
        {
            // Arrange
            EventGridEventData eventgridEventData = BuildEventGridEventData(contentType, null);

            var eventGridEvents = BuildValidEventGridEvent(null, eventTtype, eventgridEventData);

            var uri = new Uri(WebhookApiUrl, UriKind.Relative);
            var client = this.factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            // Act
            var httpResponse = await client.PostAsJsonAsync(uri, eventGridEvents);

            Assert.Equal(httpResponse.StatusCode.ToString(), System.Net.HttpStatusCode.InternalServerError.ToString());
        }

        private static EventGridEventData BuildEventGridEventData(string contentType, string contentItemId)
        {
            return new EventGridEventData
            {
                Api = $"http://test1234:7071/api/execute/{contentType}/pe0e237d-bb0e-4c46-9534-94ce2b55f98d",
                ItemId = contentItemId,
                ContentType = contentType,
                DisplayText = contentType,
            };
        }

        private static EventGridEvent[] BuildValidEventGridEvent<TModel>(string eventId, string eventType, TModel data)
        {
            var models = new EventGridEvent[]
            {
                new EventGridEvent
                {
                    Id = eventId,
                    Subject = "banners/an-integration-test-name",
                    Data = data,
                    EventType = eventType,
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                },
            };

            return models;
        }
    }
}
