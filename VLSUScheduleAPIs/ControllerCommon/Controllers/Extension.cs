using Commonlibrary.Services.Settings;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Commonlibrary.Controllers
{
    public static class Extension
    {
        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app,
         IApplicationLifetime lifetime)
        {

            var consulClient = app.ApplicationServices
                                .GetRequiredService<IConsulClient>();
            var consulConfig = app.ApplicationServices
                                .GetRequiredService<ConsulSettings>();

            var loggingFactory = app.ApplicationServices
                                .GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();


            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();


            var uri = new Uri(address);
            var registration = new AgentServiceRegistration()
            {
                ID = $"{consulConfig.ServiceId}-{uri.Port}",
                Name = consulConfig.ServiceName,
                Address = $"{uri.Scheme}://{uri.Host}",
                Port = uri.Port,
                Tags = consulConfig.Tags?.ToArray() ?? new string[0]
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}
