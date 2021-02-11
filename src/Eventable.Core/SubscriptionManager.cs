using Eventable.Core;
using System.Collections.Generic;
using System.Linq;

namespace Eventable.Core
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly Dictionary<string, List<EventSubscription>> _eventHandlers = new Dictionary<string, List<EventSubscription>>();

        public bool HasSubscriptionsForEvent<T>() where T : EventBase
        {
            var eventName = typeof(T).Name;

            return HasSubscriptionsForEvent(eventName);
        }

        public bool HasSubscriptionsForEvent(string eventName)
        {
            if (_eventHandlers.ContainsKey(eventName) && _eventHandlers[eventName].Count > 0)
                return true;

            return false;
        }

        public List<EventSubscription> GetSubscriptionsForEvent(string eventName)
        {
            if (_eventHandlers.ContainsKey(eventName) && _eventHandlers[eventName] != null)
                return _eventHandlers[eventName];

            return new List<EventSubscription>();
        }

        public void AddSubscriptionForEvent<T, H>()
            where T : EventBase
            where H : IEventHandler<T>
        {
            var eventName = typeof(T).Name;

            if (!_eventHandlers.ContainsKey(eventName))
                _eventHandlers.Add(eventName, new List<EventSubscription>());

            if (_eventHandlers[eventName].FirstOrDefault(handler => handler.HandlerType == typeof(H)) == null)
                _eventHandlers[eventName].Add(new EventSubscription(typeof(H), typeof(T)));
        }

        public void RemoveSubscriptionForEvent<T, H>()
            where T : EventBase
            where H : IEventHandler<T>
        {
            var eventName = typeof(T).Name;

            if (_eventHandlers.ContainsKey(eventName))
                _eventHandlers[eventName].RemoveAll(handler => handler.HandlerType == typeof(H));

        }
    }
}
