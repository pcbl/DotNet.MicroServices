using Services.Messages;
using System;
using WebClient.BusHelper;

namespace Services.WebClient.DataWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("WebClient - How many Alerts to send? (X to quit)");
                var input = Console.ReadLine();
                //Quit!
                if (input.ToUpper().Equals("X"))
                {
                    break;
                }
                if (int.TryParse(input, out int number))
                {
                    for (int i = 1; i <= number; i++)
                    {
                        var alert = new AlertMessage() { Text = "WebClient.Alert " + i };
                        //Publish
                        BusFactory.SendMessage(alert);

                        Console.WriteLine($"SENT: {alert}");
                    }
                }
            }
        }
    }
}
