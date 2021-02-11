using System.Threading.Tasks;

namespace Eventable.Core
{
    public interface IEventSink
    {
        Task PublishAsync(EventBase @event);
        
    }
}
