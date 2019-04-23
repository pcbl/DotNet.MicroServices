using BusHelper;
using MassTransit;
using Services.Messages;
using System;
using System.Threading;

namespace Services.DataConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server");

            var bus = BusFactory.CreateRabbitMq((host,sbc) => {
                sbc.ReceiveEndpoint(host, "alert_queue", endpoint =>
                {
                    endpoint.Handler<AlertMessage>(async context =>
                    {
                        await Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                    });
                });
            });

            //Sending our cancelation token to keep starting forever!
            //As instrcuted here
            //https://github.com/MassTransit/MassTransit/issues/1158
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            bus.StartAsync(cancelSource.Token);

            Console.WriteLine("Press Any key to leave...");

            Console.ReadLine();

            bus.StopAsync();

            Console.ReadKey();
        }
    }
}
