using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.EventMessages
{
    public delegate void MessageHandler(object sender, object e);

    public interface IMessageReceiver
    {
        Task RegisterOnMessageHandlerAndReceiveMessages(string queueName);
        Task RegisterOnMessageHandlerAndReceiveMessages(string topicName, string subscriptionName);
        event MessageHandler OnMessageReceived;

        Task CloseQueueAsync();
        ValueTask DisposeAsync();
    }
}
