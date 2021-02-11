using System;
using Eventable.Azure;
using Eventable.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Eventable.Extensions
{
    public static class EventBusBuilderExtensions 
    {
        public static EventBusBuilder UseAzureServiceBus(this EventBusBuilder builder, string connectionString,
            string topic, string subscription, Action<ServiceBusOptions> optionsAction = null)
        {
            var options = new ServiceBusOptions();
            optionsAction?.Invoke(options);

            builder.UseFactory((serviceProvider) => {
                return ServiceBusBuilder.Build(connectionString, topic, subscription, options, serviceProvider);
            });
            return builder;
        }
    }
}