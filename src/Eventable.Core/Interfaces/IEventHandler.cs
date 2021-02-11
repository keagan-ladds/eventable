using System.Threading.Tasks;

namespace Eventable.Core
{
    public interface IEventHandler<T> where T : EventBase
    {
        Task HandleAsync(T @event);
    }
}
