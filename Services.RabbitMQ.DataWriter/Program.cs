using RabbitMQ.BusHelper;
using Services.Messages;
using System;

namespace Services.RabbitMQ.DataWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("RabbitMQ - How many Alerts to send? (X to quit)");
                var input = Console.ReadLine();
                //Quit!
                if (input.ToUpper().Equals("X"))
                {
                    break;
                }
                if (int.TryParse(input, out int number))
                {
                    var channel = BusFactory.SetupChannel();
                    for (int i = 1; i <= number; i++)
                    {
                        var alert = new AlertMessage() { Text = "RabbitMQ.Alert " + i };
                        //Publish
                        BusFactory.SendMessage(alert, channel);

                        Console.WriteLine($"SENT: {alert}");
                    }
                }
            }
        }
    }
}
