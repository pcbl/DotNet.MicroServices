using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataService
{
    public class Program
    {
        public static void Main(string[] args)
        {
           /* using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect("Consul", 8500);
                    Console.WriteLine("Port open");
                }
                catch (Exception)
                {
                    Console.WriteLine("Port closed");
                }
            }*/

            var host = CreateWebHostBuilder(args).Build();
            
            #region Consul Configuration -------------------------------------------------------------------------------

            var consulClient = new ConsulClient(configuration => {
                //Check the DockerfileRunArguments on the csProj for more info
                //Sample params: --network test-net --link Consul:consul
                // consul the alias of the linkect Consul Container!
                //It is annoying: Even when I am abble to ping Consul I must add a Link to get it working correctly
                configuration.Address = new Uri("http://consul:8500"); 

            });

            // IP Parameters
            var hostDnsInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ip = hostDnsInfo.AddressList[0].ToString();
            var port = 80;  

            // Registration Parameters
            var name = "data-service";
            var id = $"{name}-{Dns.GetHostName()}:{port}";

            // Health Check Definition - check on private address within docker network.
            var tcpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(5),
                Interval = TimeSpan.FromSeconds(5),
                TCP = $"{ip}:{port}"
            };

            var appCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(5),
                Interval = TimeSpan.FromSeconds(5),
                HTTP = $"http://{ip}:{port}/api/values"
            };

            //Deregister first
            consulClient.Agent.ServiceDeregister(id).GetAwaiter().GetResult();

            // Service Registration (Gateway on public address, Healthcheck on private address)
            var registration = new AgentServiceRegistration()
            {
                Checks = new[] 
                {
                //    appCheck,
                    tcpCheck
                },
                Address = ip,
                ID = id,
                Name = name,
                Port = port,
                Tags = new[] {"api" }
            };
            #endregion

            #region Consul Registration --------------------------------------------------------------------------------
            consulClient.Agent.ServiceRegister(registration).GetAwaiter().GetResult();
            #endregion
            
            host.Run();

            #region Consul Deregistration ------------------------------------------------------------------------------
            consulClient.Agent.ServiceDeregister(id).GetAwaiter().GetResult();
            #endregion

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
