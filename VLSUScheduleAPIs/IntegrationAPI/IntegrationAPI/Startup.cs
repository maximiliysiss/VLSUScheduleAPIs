using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Consul;
using Commonlibrary.Services.Settings;
using NetServiceConnection.NetContext;
using IntegrationAPI.Services;
using Commonlibrary.Controllers;
using NetServiceConnection.Extensions;
using ControllerCommon.Services;

namespace IntegrationAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "VLSUScheduleApi",
                    Description = "VLSUScheduleApi"
                });
            });

            var consulSettings = Configuration.GetSection("consulConfig").Get<ConsulSettings>();
            var authService = Configuration.GetSection("AuthService").Get<AuthorizeService>();
            services.AddSingleton(consulSettings);
            services.AddNetContext(x => new VlsuContext(x.GetService<IConsulClient>(), x).UserAuthorization(() =>
            {

            }));
            services.AddSingleton<IConsulClient, ConsulClient>(p =>
                            new ConsulClient(consulConfig => consulConfig.Address = new Uri(consulSettings.Address)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VLSUScheduleApi");
            });

            app.RegisterWithConsul(applicationLifetime);
            app.UseMvc();
        }
    }
}
