using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Events
{
    public class BaseEvent
    {
        public EventTypes EventType { get; set; }
        public Events Event { get; set; }
    }

    public enum EventTypes
    {
        Email
    }

    public enum Events
    { 
        User_Registration,
        User_Login_OTP,
        User_Reset_Password,
        User_Broker_Invite,

        Inspection_Created,
        EditInspection_SendReminder,
        
        Company_Data_Changed,
        User_Data_Changed
    }
}
