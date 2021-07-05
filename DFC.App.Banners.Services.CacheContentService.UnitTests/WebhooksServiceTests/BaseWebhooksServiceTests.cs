using System;
using System.Collections.Generic;
using DFC.App.Banners.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.WebhooksServiceTests
{
    public abstract class BaseWebhooksServiceTests
    {
        protected BaseWebhooksServiceTests()
        {
            Logger = A.Fake<ILogger<WebhooksService>>();
            FakeEventHandlers = new List<IEventHandler>();
        }

        protected Guid ContentIdForCreate { get; } = Guid.NewGuid();

        protected Guid ContentIdForUpdate { get; } = Guid.NewGuid();

        protected Guid ContentIdForDelete { get; } = Guid.NewGuid();

        protected ILogger<WebhooksService> Logger { get; }

        protected IList<IEventHandler> FakeEventHandlers { get; }

        protected WebhooksService BuildWebhooksService()
        {
            var service = new WebhooksService(Logger, FakeEventHandlers);

            return service;
        }

        protected IEventHandler AddEventHandler(string eventHandler)
        {
            var handler = A.Fake<IEventHandler>();
            FakeEventHandlers.Add(handler);
            A.CallTo(() => handler.ProcessType).Returns(eventHandler);
            FakeEventHandlers.Add(handler);
            return handler;
        }
    }
}
