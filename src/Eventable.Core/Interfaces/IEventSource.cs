using System.Threading.Tasks;

namespace Eventable.Core
{
    public interface IEventSource
    {
        void RegisterHandler();

        Task SubscribeAsync<T, H>()
            where T : EventBase
            where H : IEventHandler<T>;

        Task UnsubscripeAsync<T, H>()
            where T : EventBase
            where H : IEventHandler<T>;
    }
}
