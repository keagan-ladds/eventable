using System;
using Eventable.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Eventable.Extensions
{
    public static class ServiceCollectionExtensions 
    {
        public static IServiceCollection AddEventBus(this IServiceCollection serviceCollection, Func<IServiceProvider, IEventBus> eventBusFactory)
        {
            return serviceCollection.AddSingleton<IEventBus>(eventBusFactory);
        }

        public static IServiceCollection AddEventBus(this IServiceCollection serviceCollection, Action<EventBusBuilder> builderAction)
        {
            var eventBusBuilder = new EventBusBuilder();
            builderAction.Invoke(eventBusBuilder);
            return serviceCollection.AddSingleton<IEventBus>((serviceProvider) => {
                return eventBusBuilder.Build(serviceProvider);
            });
        }
    }
}