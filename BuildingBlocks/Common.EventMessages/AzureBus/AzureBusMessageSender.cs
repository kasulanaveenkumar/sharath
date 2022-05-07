using Azure.Messaging.ServiceBus;
using Common.Models.Events;
using Microsoft.Azure.ServiceBus;
//using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Text.Json;

namespace Common.EventMessages.AzureBus
{
    public class AzureBusMessageSender : IMessageSender
    {
        ServiceBusClient client = null;
        ServiceBusSender sender = null;

        ITopicClient topicClient = null;

        #region Constructor

        public AzureBusMessageSender()
        {
            client = new ServiceBusClient(MessageConstants.AzureBusConnectionString);
        }

        #endregion

        #region Send Message

        public async void SendMessage(string queueName, object model)
        {
            sender = client.CreateSender(queueName);

            var msgBody = JsonSerializer.Serialize(model);

            await sender.SendMessageAsync(new ServiceBusMessage(msgBody));
        }

        public async void SendMessageToTopic(string topicName, object model)
        {
            topicClient = new TopicClient(MessageConstants.TopicConnectionString, topicName);

            var msgBody = JsonSerializer.Serialize(model);

            var message = new Message(Encoding.UTF8.GetBytes(msgBody));

            await topicClient.SendAsync(message);
        }

        #endregion
    }
}
