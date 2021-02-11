using System;

namespace Eventable.Core
{
    public abstract class EventBase
    {
        public Guid EventId { get; set; }
        public DateTimeOffset EventTime { get; set; }

        public string SessionId { get; set; }

        public EventBase()
        {
            EventId = Guid.NewGuid();
            EventTime = DateTimeOffset.UtcNow;
        }
    }
}
