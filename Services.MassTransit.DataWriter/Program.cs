using BusHelper;
using MassTransit;
using Services.Messages;
using System;
using System.Threading;

namespace Services.DataWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("How many Alerts to send? (X to quit)");
                var input = Console.ReadLine();
                //Quit!
                if (input.ToUpper().Equals("X"))
                {
                    break;
                }
                if (int.TryParse(input, out int number))
                {
                    //Connect and start the bus
                    var bus = BusFactory.CreateRabbitMq();

                    //Sending our cancelation token to keep starting forever!
                    //As instrcuted here
                    //https://github.com/MassTransit/MassTransit/issues/1158
                    CancellationTokenSource cancelSource = new CancellationTokenSource();
                    bus.StartAsync(cancelSource.Token);

                    for (int i = 1; i <= number; i++)
                    {
                        var alert = new AlertMessage() { Text = "Alert "+ i };

                        
                        //Publish
                        var publishTask = bus.Publish(alert);

                        Console.WriteLine($"{alert.Text} - SENT");
                    }

                    bus.StopAsync();
                }
            }
        }
    }
}
