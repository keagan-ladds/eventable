using System;
using System.Collections.Generic;

namespace Eventable.Core
{
    public class EventSubscription 
    {
        public Type HandlerType {get; set;}
        public Type EventType {get; set;}

        public EventSubscription(Type handlerType, Type eventType)
        {
            HandlerType = handlerType;
            EventType = eventType;
        }
    }

    public interface ISubscriptionManager
    {
        bool HasSubscriptionsForEvent<T>() where T : EventBase;
        bool HasSubscriptionsForEvent(string eventName);

        void AddSubscriptionForEvent<T, H>()
            where T : EventBase
            where H : IEventHandler<T>;

        void RemoveSubscriptionForEvent<T, H>()
        where T : EventBase
        where H : IEventHandler<T>;

        List<EventSubscription> GetSubscriptionsForEvent(string eventName);
    }
}