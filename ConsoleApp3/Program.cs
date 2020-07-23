using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Web;

namespace ConsoleApp3
{
    class Program
    {
        static ISubscriptionClient subscriptionClient;

        static void Main(string[] args)
        {
            Console.WriteLine("Listner started.");
            Deadletter().GetAwaiter().GetResult();
        }

        static async Task Deadletter()
        {
            try
            {
                string ServiceBusConnectionString = "Endpoint=sb://pricechange.servicebus.windows.net/;SharedAccessKeyName=cartitempricechange;SharedAccessKey=zOK+zZifCv4xX4f0af4Bctrg6tXeLKvwD9vhBT9YlZ8=";
                string TopicName = "pricechange-topic";
                string SubscriptionName = "pricechange-topic-subscription";
                MessagingFactory factory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
                string path = Microsoft.ServiceBus.Messaging.SubscriptionClient.FormatDeadLetterPath(TopicName, SubscriptionName);
                Microsoft.ServiceBus.Messaging.MessageReceiver deadletterReceiver = factory.CreateMessageReceiver(path);

                BrokeredMessage msg = deadletterReceiver.Receive();
                if (msg != null)
                {
                    var messageBody = new StreamReader(msg.GetBody<Stream>(), Encoding.UTF8).ReadToEnd();
                    var receivedMessage = JsonConvert.DeserializeObject<ServiceBusMessage>(messageBody);

                    Console.WriteLine($"Received message: UserInfo:{receivedMessage.Id}");
                    Console.ReadKey();
                    msg.Complete();
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //static async Task MainAsync()
        //{

        //    string ServiceBusConnectionString = "Endpoint=sb://pricechange.servicebus.windows.net/;SharedAccessKeyName=cartitempricechange;SharedAccessKey=zOK+zZifCv4xX4f0af4Bctrg6tXeLKvwD9vhBT9YlZ8=";
        //    string TopicName = "pricechange-topic";
        //    string SubscriptionName = "pricechange-topic-subscription";


        //    subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

        //    // Register subscription message handler and receive messages in a loop.
        //    RegisterOnMessageHandlerAndReceiveMessages();

        //    Console.ReadKey();

        //    await subscriptionClient.CloseAsync();
        //}

        //static void RegisterOnMessageHandlerAndReceiveMessages()
        //{
        //    var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler);

        //    // Register the function that processes messages.
        //    subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        //}

        //static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        //{
        //    var messageBody = Encoding.UTF8.GetString(message.Body);

        //    var serviceBusMessage = JsonConvert.DeserializeObject<ServiceBusMessage>(messageBody);

        //    Console.WriteLine($"Received message: UserInfo:{Encoding.UTF8.GetString(message.Body)}");

        //    await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        //}

        //static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        //{
        //    var exception = exceptionReceivedEventArgs.Exception;

        //    return Task.CompletedTask;
        //}

        public class ServiceBusMessage
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Content { get; set; }
        }
    }
}
