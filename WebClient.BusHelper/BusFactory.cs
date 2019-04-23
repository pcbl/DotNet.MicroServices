using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace WebClient.BusHelper
{
    public static class BusFactory
    {
        public const string exchangeName = "topic.logs";
        public const string exchangeQueueName = "topic.logs.queue";
        public const string routingKey = "test.app";
        public const string baseUrl = "http://localhost:15672";
        private const string user = "admin";
        private const string pwd= "test";

        public static HttpClient SetupClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            var byteArray = Encoding.ASCII.GetBytes($"{user}:{pwd}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return client;
        }

        public static void SendMessage<MessageType>(MessageType message)
        {
            try
            {
                var client = SetupClient();
                try
                {
                    Console.Write($"Sending '{message}'@{exchangeName}...");
                    byte[] messageBodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    client.PostAsync($"{baseUrl}/api/exchanges/%2F/{exchangeName}/publish",
                        new StringContent(JsonConvert.SerializeObject(
                            new{
                                properties = new { },
                                routing_key = routingKey,
                                payload = JsonConvert.SerializeObject(message),
                                payload_encoding = "string"}
                            )));                                        
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

        

        public static void Listen<MessageType>(Action<MessageType, ServerResponse> callback)
        {
            var client = SetupClient();
            var post = client.PostAsync($"{baseUrl}/api/queues/%2F/{exchangeQueueName}/get",
                       new StringContent(JsonConvert.SerializeObject(
                           new
                           {
                               count = 1,
                               ackmode = "ack_requeue_false",
                               encoding = "auto"
                           }
                           )));

            post.Wait();
            if (post.IsCompletedSuccessfully)
            {
                var fullPayload = post.Result.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<ServerResponse[]>(fullPayload);
                if (response.Length > 0)
                {
                    var castedPayload = JsonConvert.DeserializeObject<MessageType>(response[0].payload);
                    callback(castedPayload, response[0]);
                }
            }
        }
    }
}
