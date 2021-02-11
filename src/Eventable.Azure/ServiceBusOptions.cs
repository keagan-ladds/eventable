namespace Eventable.Azure
{
    public class ServiceBusOptions
    {
        public int MaxConcurrentCalls {get; set;} = 1;

        public bool UseSessions {get; set;} = false;

        public bool RemoveDefaultRule {get; set;} = false;
    }
}