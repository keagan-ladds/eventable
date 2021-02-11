using Microsoft.Extensions.DependencyInjection;
using System;

namespace Eventable.Core
{
    public class ServiceProviderResolver : IHandlerResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;
        public ServiceProviderResolver(IServiceProvider serviceProvider)
        {
            _serviceScope = serviceProvider.CreateScope();
            _serviceProvider = _serviceScope.ServiceProvider;
        }

        public void Dispose()
        {
            _serviceScope?.Dispose();
        }

        public object ResolveHandler(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}
