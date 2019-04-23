using Services.Messages;
using System;
using WebClient.BusHelper;

namespace Services.WebClient.DataConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("(WebClient) Listening...");
            while(true)
            {
                BusFactory.Listen<AlertMessage>((payload,response)=>{
                    Console.WriteLine($"RECEIVED@{response.routing_key}: {payload}");
                });
            }
        }
    }
}
