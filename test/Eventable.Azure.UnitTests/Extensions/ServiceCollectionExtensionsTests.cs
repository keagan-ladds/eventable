using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using Eventable.Extensions;
using Moq;
using Eventable.Core;

namespace Eventable.Azure.UnitTests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        [Test]
        public void Test()
        {
            Mock<IServiceCollection> serviceCollection = new Mock<IServiceCollection>();
            serviceCollection.Setup(collection => collection.AddSingleton<IEventBus>()).Verifiable();
            serviceCollection.Setup(s => s.AddSingleton<IEventBus>());

            var serviceProvider = serviceCollection.Object
                .AddEventBus(builder => {
                    builder.UseAzureServiceBus("connection-string", "topic", "subscription", options => {
                        options.MaxConcurrentCalls = 1;
                    });
                });
        }
    }
}