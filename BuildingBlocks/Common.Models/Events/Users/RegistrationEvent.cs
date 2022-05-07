namespace Common.Models.Events.Users
{
    public class RegistrationEvent : BaseEvent
    {
        public RegistrationEvent()
        {
            EventType = EventTypes.Email;
            Event = Events.User_Registration;
        }

        public string ToEmail { get; set; }
        public string ActivationLink { get; set; }
    }
}
