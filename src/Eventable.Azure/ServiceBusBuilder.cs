using System;
using Eventable.Core;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Eventable.Azure
{
    public class ServiceBusBuilder
    {
        public static ServiceBusEventBus Build(string connectionString, string topic, string subscription, ServiceBusOptions options,
            IServiceProvider serviceProvider) 
        {
            var topicClient = new TopicClient(connectionString, topic);
            var subscriptionClient = new SubscriptionClient(connectionString, topic, subscription);
            var subscriptionManager = new SubscriptionManager();
            var providerResolver = new ServiceProviderResolver(serviceProvider);
            var logger = serviceProvider.GetService<ILogger<ServiceBusEventBus>>();

            return new ServiceBusEventBus(subscriptionClient, topicClient, options, 
                subscriptionManager, providerResolver, logger);
        }
    }
}