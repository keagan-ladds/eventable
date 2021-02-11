using System;
using Eventable.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Eventable.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void UseEventBus(this IServiceProvider serviceProvider, Action<IEventBus> options) 
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            options.Invoke(eventBus);
            eventBus.RegisterHandler();
        }
    }
}