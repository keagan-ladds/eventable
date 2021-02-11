using System;
using System.Text;
using System.Threading.Tasks;
using Eventable.Core;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Eventable.Azure.UnitTests")]
namespace Eventable.Azure
{
    public class ServiceBusEventBus : IEventBus
    {
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly ITopicClient _topicClient;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IHandlerResolver _handlerResolver;

        private readonly ServiceBusOptions _serviceBusOptions;

        private readonly ILogger _logger;

        internal ServiceBusEventBus(ISubscriptionClient subscriptionClient, ITopicClient topicClient, ServiceBusOptions serviceBusOptions,
            ISubscriptionManager subscriptionManager, IHandlerResolver handlerResolver, ILogger<ServiceBusEventBus> logger)
        {
            _subscriptionClient = subscriptionClient ?? throw new ArgumentNullException(nameof(subscriptionClient));
            _topicClient = topicClient ?? throw new ArgumentNullException(nameof(topicClient));
            _serviceBusOptions = serviceBusOptions ?? throw new ArgumentNullException(nameof(serviceBusOptions));
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
            _handlerResolver = handlerResolver ?? throw new ArgumentNullException(nameof(handlerResolver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task PublishAsync(EventBase @event)
        {
            var eventName = @event.GetType().Name;
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = body,
                Label = eventName,
                SessionId = @event.SessionId
            };

            await _topicClient.SendAsync(message);
        }

        public void RegisterHandler()
        {
            if (_serviceBusOptions.UseSessions)
                RegisterSubscriptionClientSessionHandler();
            else
                RegisterSubscriptionClientMessageHandler();
        }

        private void RegisterSubscriptionClientSessionHandler() 
        {
            _subscriptionClient.RegisterSessionHandler(
                    async (session, message, token) =>
                    {
                        var eventName = message.Label;
                        var messageData = Encoding.UTF8.GetString(message.Body);

                        if (await ProcessEvent(eventName, messageData))
                        {
                            await session.CompleteAsync(message.SystemProperties.LockToken);
                        }
                    },
                    ExceptionReceivedHandler);
        }

        private void RegisterSubscriptionClientMessageHandler()
        {
            _subscriptionClient.RegisterMessageHandler(
                    async (message, token) =>
                    {
                        var eventName = message.Label;
                        var messageData = Encoding.UTF8.GetString(message.Body);

                        if (await ProcessEvent(eventName, messageData))
                        {
                            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                        }
                    },
                    new MessageHandlerOptions(ExceptionReceivedHandler) 
                    { MaxConcurrentCalls = _serviceBusOptions.MaxConcurrentCalls, AutoComplete = false });

        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            // TODO : Logging

            return Task.CompletedTask;
        }

        private async Task<bool> ProcessEvent(string eventName, string message)
        {
            var processed = false;
            if (_subscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptions = _subscriptionManager.GetSubscriptionsForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    var handler = _handlerResolver?.ResolveHandler(subscription.HandlerType);
                    if (handler == null) continue;

                    var integrationEvent = JsonConvert.DeserializeObject(message, subscription.EventType);
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(subscription.EventType);
                    await (Task)concreteType.GetMethod("HandleAsync").Invoke(handler, new object[] { integrationEvent });
                    processed = true;
                }
            }

            return processed;
        }

        public async Task SubscribeAsync<T, H>()
            where T : EventBase
            where H : IEventHandler<T>
        {
            var eventName = typeof(T).Name;

            var containsKey = _subscriptionManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                try
                {
                    await _subscriptionClient.AddRuleAsync(new RuleDescription
                    {
                        Filter = new CorrelationFilter { Label = eventName },
                        Name = eventName
                    });
                }
                catch (ServiceBusException ex)
                {
                    //TODO: Logging
                }
            }

            _subscriptionManager.AddSubscriptionForEvent<T, H>();
        }

        public async Task UnsubscripeAsync<T, H>()
            where T : EventBase
            where H : IEventHandler<T>
        {
            var eventName = typeof(T).Name;

            try
            {
                await _subscriptionClient.RemoveRuleAsync(eventName);
            }
            catch (MessagingEntityNotFoundException)
            {
                // TODO: Logging
            }

            _subscriptionManager.RemoveSubscriptionForEvent<T, H>();
        }
    }
}