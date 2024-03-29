﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Enums;
using DFC.App.Banners.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

namespace DFC.App.Banners.Controllers
{
    [Route("api/webhook")]
    public class WebhooksController : Controller
    {
        private readonly Dictionary<string, WebhookCacheOperation> acceptedEventTypes = new Dictionary<string, WebhookCacheOperation>
        {
            { "draft", WebhookCacheOperation.CreateOrUpdate },
            { "published", WebhookCacheOperation.CreateOrUpdate },
            { "draft-discarded", WebhookCacheOperation.Delete },
            { "unpublished", WebhookCacheOperation.Delete },
            { "deleted", WebhookCacheOperation.Delete },
        };

        private readonly ILogger<WebhooksController> logger;
        private readonly IWebhooksService webhookService;

        public WebhooksController(
            ILogger<WebhooksController> logger,
            IWebhooksService webhookService)
        {
            this.logger = logger;
            this.webhookService = webhookService;
        }

        [HttpPost]
        [Route("ReceiveEvents")]
        public async Task<IActionResult> ReceiveEvents()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            string requestContent = await reader.ReadToEndAsync();
            logger.LogInformation($"Received events: {requestContent}");

            var eventGridSubscriber = new EventGridSubscriber();
            foreach (var key in acceptedEventTypes.Keys)
            {
                eventGridSubscriber.AddOrUpdateCustomEventMapping(key, typeof(EventGridEventData));
            }

            var eventGridEvents = eventGridSubscriber.DeserializeEventGridEvents(requestContent);

            foreach (var eventGridEvent in eventGridEvents)
            {
                if (!Guid.TryParse(eventGridEvent.Id, out Guid eventId))
                {
                    throw new InvalidDataException($"Invalid Guid for EventGridEvent.Id '{eventGridEvent.Id}'");
                }

                if (eventGridEvent.Data is SubscriptionValidationEventData subscriptionValidationEventData)
                {
                    logger.LogInformation($"Got SubscriptionValidation event data, validationCode: {subscriptionValidationEventData!.ValidationCode},  validationUrl: {subscriptionValidationEventData.ValidationUrl}, topic: {eventGridEvent.Topic}");

                    // Do any additional validation (as required) such as validating that the Azure resource ID of the topic matches
                    // the expected topic and then return back the below response
                    var responseData = new SubscriptionValidationResponse()
                    {
                        ValidationResponse = subscriptionValidationEventData.ValidationCode,
                    };

                    return Ok(responseData);
                }
                else if (eventGridEvent.Data is EventGridEventData eventGridEventData)
                {
                    if (!Guid.TryParse(eventGridEventData.ItemId, out Guid contentId))
                    {
                        throw new InvalidDataException($"Invalid Guid for EventGridEvent.Data.ItemId '{eventGridEventData.ItemId}'");
                    }

                    var cacheOperation = acceptedEventTypes[eventGridEvent.EventType];

                    logger.LogInformation($"Got Event Id: {eventId}: {eventGridEvent.EventType}: Cache operation: {cacheOperation} {eventGridEventData.Api}");

                    await webhookService.ProcessMessageAsync(cacheOperation, eventId, contentId, eventGridEventData.Api!, eventGridEventData.ContentType!);
                }
                else
                {
                    throw new InvalidDataException($"Invalid event type '{eventGridEvent.EventType}' received for Event Id: {eventId}, should be one of '{string.Join(",", acceptedEventTypes.Keys)}'");
                }
            }

            return Ok();
        }
    }
}