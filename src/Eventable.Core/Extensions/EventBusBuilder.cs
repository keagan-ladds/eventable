using System;
using Eventable.Core;

namespace Eventable.Extensions 
{
    public class EventBusBuilder
    {
        private Func<IServiceProvider, IEventBus> _eventBusFactory;

        public EventBusBuilder UseFactory(Func<IServiceProvider, IEventBus> factory) 
        {
            _eventBusFactory = factory;
            return this;
        }

        internal IEventBus Build(IServiceProvider serviceProvider) 
        {
            return _eventBusFactory.Invoke(serviceProvider);
        }
    }
}