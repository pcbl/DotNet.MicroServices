using RabbitMQ.BusHelper;
using Services.Messages;
using System;

namespace Services.RabbitMQ.DataConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = BusFactory.SetupChannel();
            //Configure TOPIC!
            BusFactory.SetupTopic(channel);
            Console.WriteLine("Server Up and running...");
            BusFactory.Listen<AlertMessage>((data,ea) =>
            {
                Console.WriteLine($"RECEIVED@{ea.RoutingKey}: {data}");
            });
        }
    }
}
