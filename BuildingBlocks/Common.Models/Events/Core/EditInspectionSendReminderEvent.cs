using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Events.Core
{
    public class EditInspectionSendReminderEvent : BaseEvent
    {
        public EditInspectionSendReminderEvent()
        {
            EventType = EventTypes.Email;
            Event = Events.EditInspection_SendReminder;
        }

        public string ToEmail { get; set; }

        public string ReminderMessage { get; set; }
    }
}
