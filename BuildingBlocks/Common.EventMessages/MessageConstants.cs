using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.EventMessages
{
    public class MessageConstants
    {
        public static string AzureBusConnectionString = "Endpoint=sb://fireflash-sb-aus-dev.servicebus.windows.net/;SharedAccessKeyName=Notification;SharedAccessKey=P8vOLgxRm/cZorPBoWcvux0bSXojXDC0i40XK+rbMD4=;EntityPath=queue_notification;";

        public const string QueueName_Notification = "queue_notification";

        public static string TopicConnectionString = "Endpoint=sb://fireflash-sb-aus-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=6XXOta11Dymb17stMgvo6XZWiR0FEz1CkLKdISMgQCo=";

        public const string TopicName = "userdata";

        public const string SubscriptionName = "fireflashservicebussub";
    }
}
