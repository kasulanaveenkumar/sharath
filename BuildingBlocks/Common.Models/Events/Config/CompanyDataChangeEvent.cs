using System;

namespace Common.Models.Events.Config
{
    public class CompanyDataChangeEvent : BaseEvent
    {
        public CompanyDataChangeEvent()
        {
            Event = Events.Company_Data_Changed;
        }

        public string CompanyGuid { get; set; }

        public string CompanyName { get; set; }

        public Int64 CompanyTypeId { get; set; }

        public bool IsPayer { get; set; }
    }
}
