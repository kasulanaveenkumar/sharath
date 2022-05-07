namespace Common.Models.Events.Users
{
    public class BrokerInviteUserEvent : BaseEvent
    {
        public BrokerInviteUserEvent()
        {
            EventType = EventTypes.Email;
            Event = Events.User_Broker_Invite;
        }

        public string BrokerName { get; set; }

        public string CompanyName { get; set; }

        public string ToEmail { get; set; }

        public string ActivationLink { get; set; }
    }
}
