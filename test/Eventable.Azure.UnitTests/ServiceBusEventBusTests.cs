using System;
using System.Threading.Tasks;
using Eventable.Azure.UnitTests.Events;
using Eventable.Core;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Eventable.Azure.UnitTests
{
    public class ServiceBusEventBusTests
    {
        [Test]
        public void CtorThrowsOnNull()
        {
            ISubscriptionClient subscriptionClient = new Mock<ISubscriptionClient>().Object;
            ITopicClient topicClient = new Mock<ITopicClient>().Object;
            ServiceBusOptions serviceBusOptions = new Mock<ServiceBusOptions>().Object;
            ISubscriptionManager subscriptionManager = new Mock<ISubscriptionManager>().Object;
            IHandlerResolver handlerResolver = new Mock<IHandlerResolver>().Object;
            ILogger<ServiceBusEventBus> logger = new Mock<ILogger<ServiceBusEventBus>>().Object;

            Assert.Throws<ArgumentNullException>(() => new ServiceBusEventBus(null, topicClient, serviceBusOptions, subscriptionManager, handlerResolver, logger));
            Assert.Throws<ArgumentNullException>(() => new ServiceBusEventBus(subscriptionClient, null, serviceBusOptions, subscriptionManager, handlerResolver, logger));
            Assert.Throws<ArgumentNullException>(() => new ServiceBusEventBus(subscriptionClient, topicClient, null, subscriptionManager, handlerResolver, logger));
            Assert.Throws<ArgumentNullException>(() => new ServiceBusEventBus(subscriptionClient, topicClient, serviceBusOptions, null, handlerResolver, logger));
            Assert.Throws<ArgumentNullException>(() => new ServiceBusEventBus(subscriptionClient, topicClient, serviceBusOptions, subscriptionManager, null, logger));
            Assert.Throws<ArgumentNullException>(() => new ServiceBusEventBus(subscriptionClient, topicClient, serviceBusOptions, subscriptionManager, handlerResolver, null));
        }

        [Test]
        public async Task PublishAsyncMessageFieldsSet() 
        {
            var subscriptionClient = new Mock<ISubscriptionClient>();
            var topicClient = new Mock<ITopicClient>();
            ServiceBusOptions serviceBusOptions = new ServiceBusOptions();
            var subscriptionManager = new Mock<ISubscriptionManager>();
            var handlerResolver = new Mock<IHandlerResolver>();
            var logger = new Mock<ILogger<ServiceBusEventBus>>();

            topicClient.Setup(client => client.SendAsync(It.IsAny<Message>())).Callback<Message>((message) => {
                Assert.AreEqual("BasicEvent", message.Label);
                Assert.IsNotEmpty(message.MessageId);
            });

            var eventBus = new ServiceBusEventBus(subscriptionClient.Object, topicClient.Object, serviceBusOptions,
                subscriptionManager.Object, handlerResolver.Object, logger.Object);

            var @event = new BasicEvent();
            
            await eventBus.PublishAsync(@event);

            Mock.Verify(topicClient);
        }
    }
}