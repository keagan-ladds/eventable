using System;

namespace Eventable.Core
{
    public interface IHandlerResolver : IDisposable
    {
        object ResolveHandler(Type type);
    }
}
