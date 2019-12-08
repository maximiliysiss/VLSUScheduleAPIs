using Commonlibrary.Models;
using Commonlibrary.Services.Settings;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Commonlibrary.Controllers
{
    public static class Extension
    {
        public static async Task<string> LoginService(this IApplicationBuilder app, string login, string password)
        {
            var consulClient = app.ApplicationServices
                                .GetRequiredService<IConsulClient>();
            var services = consulClient.Agent.Services().Result.Response.FirstOrDefault(x => x.Value.Service.Contains("auth", StringComparison.OrdinalIgnoreCase));
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var res = await client.PostAsJsonAsync($"http://{services.Value.Address}/api/auth/login", new { Login = login, Password = password });
                var strLogin = await res.Content.ReadAsStringAsync();
                return $"Bearer {JObject.Parse(strLogin)["accessToken"].Value<string>()}";
            }
        }

        public static int? UserId(this ControllerBase controllerBase)
        {
            var user = controllerBase.User.Claims.FirstOrDefault(x => x.Type == "User");
            if (user == null)
                return null;
            return int.Parse(user.Value);
        }

        public static UserType? UserType(this ControllerBase controllerBase)
        {
            var user = controllerBase.User.Claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultRoleClaimType);
            if (user == null)
                return null;
            return (UserType)Enum.Parse(typeof(UserType), user.Value);
        }

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IApplicationLifetime lifetime, string overrideAddress = null)
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

            var port = address.Substring(address.LastIndexOf(":") + 1);
            if (port.Last() == '/')
                port = port.Substring(0, port.Length - 1);

            var registration = new AgentServiceRegistration()
            {
                ID = $"{consulConfig.ServiceId}",
                Name = consulConfig.ServiceName,
                Address = overrideAddress ?? address,
                Port = int.Parse(port),
                Tags = consulConfig.Tags?.ToArray() ?? new string[0]
            };

            logger.LogInformation("Registered with Consul");

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
