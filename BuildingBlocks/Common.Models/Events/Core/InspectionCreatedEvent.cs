namespace Common.Models.Events.Core
{
    public class InspectionCreatedEvent : BaseEvent
    {
        public InspectionCreatedEvent()
        {
            EventType = EventTypes.Email;
            Event = Events.Inspection_Created;
        }

        public string ToEmail { get; set; }

        public string SellerName { get; set; }
        public string MobileWebAppLink { get; set; }
        public string InspectionID { get; set; }
        public string Documents { get; set; }
        public string AssetType { get; set; }
        public string BrokerName { get; set; }
        public string BrokerEmail { get; set; }
        public string BrokerMobile { get; set; }
    }
}
