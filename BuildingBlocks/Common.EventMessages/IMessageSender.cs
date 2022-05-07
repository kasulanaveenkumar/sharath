using Common.Models.Events;

namespace Common.EventMessages
{
    public interface IMessageSender
    {
        void SendMessage(string queueName, object msg);

        void SendMessageToTopic(string topicName, object msg);
    }
}
