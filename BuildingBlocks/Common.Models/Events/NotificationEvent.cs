using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Events
{
    public class NotificationEvent : BaseEvent
    {
        public NotificationEvent()
        {
            EventType = EventTypes.Email;
        }

        public List<string> ToEmail { get; set; }
    }
}
