using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Events.Users
{
    public class ResetPasswordEvent : BaseEvent
    {
        public ResetPasswordEvent()
        {
            EventType = EventTypes.Email;
            Event = Events.User_Reset_Password;
        }

        public string ToEmail { get; set; }

        public string Name { get; set; }
        public string SurName { get; set; }

        public string Link { get; set; }
    }
}
