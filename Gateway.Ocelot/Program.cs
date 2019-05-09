using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
namespace Gateway.Ocelot
{
    public class Program
    {
        public static void Main(string[] args)
        {

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
            var name = "ocelot-gateway";
            var id = $"{name}-{Dns.GetHostName()}:{port}";

            // Health Check Definition - check on private address within docker network.
            var tcpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(5),
                Interval = TimeSpan.FromSeconds(5),
                TCP = $"{ip}:{port}"
            };

            //Deregister first
            consulClient.Agent.ServiceDeregister(id).GetAwaiter().GetResult();

            // Service Registration (Gateway on public address, Healthcheck on private address)
            var registration = new AgentServiceRegistration()
            {
                Checks = new[]
                {
                    tcpCheck
                },
                Address = ip,
                ID = id,
                Name = name,
                Port = port,
                Tags = new[] { "gateway" }
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
                 .UseKestrel(options =>
                 {
                     options.Limits.MaxConcurrentConnections = 500;
                     options.Limits.MaxConcurrentUpgradedConnections = 500;
                     options.Limits.MaxRequestBodySize = 10 * 1024;
                     options.Limits.MinRequestBodyDataRate =
                         new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                     options.Limits.MinResponseDataRate =
                         new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                 })
                .UseContentRoot(Directory.GetCurrentDirectory())                
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddJsonFile("ocelot.json", false, true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices(services =>
                {
                    services
                        .AddOcelot()
                        .AddConsul()
                        .AddConfigStoredInConsul();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseIISIntegration()
                .Configure(app =>
                {
                    app.UseOcelot().Wait();
                });

    }
}
