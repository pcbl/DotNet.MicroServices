using Consul;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Packages.Configuration
{
    public static class KeyValue
    {
        private static ConsulClient CreateClient()
        {
            return new ConsulClient(configuration =>
            {
                //Check the DockerfileRunArguments on the csProj for more info
                //Sample params: --network test-net --link Consul:consul
                // consul the alias of the linkect Consul Container!
                //It is annoying: Even when I am abble to ping Consul I must add a Link to get it working correctly
                configuration.Address = new Uri("http://consul:8500");

            });
        }
        public static Task<WriteResult<bool>> Add<T>(string key, T value)
        {
            using (var client = CreateClient())
            {
                var putPair = new KVPair(key)
                {
                    Value = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value))
                };
                
                return client.KV.Put(putPair); 
            }
        }

        public static async Task<T> Get<T>(string key, T defaultValue=default(T))
        {
            try
            {
                using (var client = CreateClient())
                {                   
                    var getPair = await client.KV.Get(key);

                    if (getPair.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return defaultValue;

                    var toReturn = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length));             
                    return toReturn;
                }
            }
            catch
            {
                return defaultValue;
            }

        }

    }
}
