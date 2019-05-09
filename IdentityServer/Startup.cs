using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Identity Server Tutorial:
            //https://www.scottbrady91.com/Identity-Server/Getting-Started-with-IdentityServer-4
            services.AddIdentityServer((config)=>
            { config.IssuerUri = "http://identityserver";                
            })
                .AddInMemoryClients(Store.Clients.Get())
                .AddInMemoryIdentityResources(Store.Resources.GetIdentityResources())
                .AddInMemoryApiResources(Store.Resources.GetApiResources())
                .AddCorsPolicyService<InMemoryCorsPolicyService>()
                .AddTestUsers(Store.Users.Get())
                .AddDeveloperSigningCredential();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Enabling Cors
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
