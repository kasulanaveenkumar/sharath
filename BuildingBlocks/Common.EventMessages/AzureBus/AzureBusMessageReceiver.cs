using Azure.Messaging.ServiceBus;
using Common.Models.Events;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Common.EventMessages.AzureBus
{
    public class AzureBusMessageReceiver : IMessageReceiver
    {
        public event MessageHandler OnMessageReceived;

        ServiceBusClient client = null;
        ServiceBusProcessor processor = null;

        #region Constructor

        public AzureBusMessageReceiver()
        {
            // Create the client object that will be used to create sender and receiver objects
            client = new ServiceBusClient(MessageConstants.AzureBusConnectionString);
        }

        #endregion

        #region Register Message Handler

        public async Task RegisterOnMessageHandlerAndReceiveMessages(string queueName)
        {
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += MessageHandler;

                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Handle Exception
            }
            finally
            {
            }
        }

        public async Task RegisterOnMessageHandlerAndReceiveMessages(string topicName, string subscriptionName)
        {
            client = new ServiceBusClient(MessageConstants.TopicConnectionString);

            processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += MessageHandler;

                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Handle Exception
            }
            finally
            {
            }
        }

        #endregion

        #region Message Handler

        // handle received messages
        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            //BaseEvent eventData = JsonSerializer.Deserialize<BaseEvent>(args.Message.Body);
            OnMessageReceived(this, args.Message.Body.ToObjectFromJson<object>());

            // complete the message. messages is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message).ConfigureAwait(false);
        }

        #endregion

        #region Error Handler

        // handle any errors when receiving messages
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        #endregion        

        #region Close Queue

        public async Task CloseQueueAsync()
        {
            await processor.CloseAsync().ConfigureAwait(false);
        }

        #endregion

        #region Dispose 

        public async ValueTask DisposeAsync()
        {
            if (processor != null)
            {
                await processor.DisposeAsync().ConfigureAwait(false);
            }

            if (client != null)
            {
                await client.DisposeAsync().ConfigureAwait(false);
            }
        }

        #endregion
    }
}
