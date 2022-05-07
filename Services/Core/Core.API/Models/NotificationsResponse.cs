using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class NotificationsResponse
    {
        public string NotificationsGuid { get; set; }

        public string EventDescription { get; set; }

        public bool IsSelected { get; set; }
    }
}
