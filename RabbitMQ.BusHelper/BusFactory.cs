using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.BusHelper
{
    public static class BusFactory
    {

        public static IModel SetupChannel()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://admin:test@localhost:5672/");

            Console.WriteLine("Connecting...");
            IConnection conn = factory.CreateConnection();
            Console.WriteLine("Connected.");

            return conn.CreateModel();

        }

        public const string exchangeName = "topic.logs";
        public const string exchangeQueueName = "topic.logs.queue";
        public const string routingKey = "test.app";

        public static void SetupTopic(IModel channel)
        {
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

            var queue = channel.QueueDeclare(exchangeQueueName, true, false, false, null).QueueName;

            channel.QueueBind(queue: queue,
                              exchange: exchangeName,
                              routingKey: routingKey);
        }


        public static void SendMessage<MessageType>(MessageType message, IModel channel=null)
        {
            try
            {
                if(channel==null)
                    channel = SetupChannel();
                try
                {
                    Console.Write($"Sending '{message}'@{exchangeName}...");
                    byte[] messageBodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
                    Console.WriteLine($" - OK");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" - NOT OK ({ex.Message})");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Listen<MessageType>(Action<MessageType,BasicDeliverEventArgs> callback)
        {
            IModel channel = SetupChannel();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var messageString = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                var message = JsonConvert.DeserializeObject<MessageType>(messageString);
                callback(message,ea);
            };
            channel.BasicConsume(queue: exchangeQueueName, autoAck: true, consumer: consumer);
        }
    }
}
