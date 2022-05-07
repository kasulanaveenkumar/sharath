using Azure.Messaging.ServiceBus;

namespace Common.EventMessages
{
    public class MessageSender
    {
        private static string connectionString = "Endpoint=sb://winmin.servicebus.windows.net/;SharedAccessKeyName=policyff;SharedAccessKey=IyEY5kEjiKvedtyzw+x2DiW6wTL1mXlUnkwvwmdbuxw=;EntityPath=eventsqueue;";

        private static string queueName = "eventsqueue";

        public static async void SendMessage()
        {
            ServiceBusClient client = null;
            ServiceBusSender sender = null;

            try
            {
                client = new ServiceBusClient(connectionString);
                sender = client.CreateSender(queueName);

                await sender.SendMessageAsync(new ServiceBusMessage("First message"));
            }
            finally
            {
                if (sender != null)
                    await sender.DisposeAsync();

                if (client != null)
                    await client.DisposeAsync();
            }
        }
    }
}
