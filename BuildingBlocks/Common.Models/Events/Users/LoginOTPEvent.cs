using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Events.Users
{
    public class LoginOTPEvent : BaseEvent
    {
        public LoginOTPEvent()
        {
            EventType = EventTypes.Email;
            Event = Events.User_Login_OTP;
        }

        public string ToEmail { get; set; }
        
        public string Name { get; set; }
        public string SurName { get; set; }

        public string OTP { get; set; }
    }
}
