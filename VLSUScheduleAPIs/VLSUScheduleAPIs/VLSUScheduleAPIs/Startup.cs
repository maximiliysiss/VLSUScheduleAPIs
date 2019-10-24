﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Consul;
using Commonlibrary.Services.Settings;
using System;
using System.Reflection.PortableExecutable;
using VLSUScheduleAPIs.Services;
using Commonlibrary.Controllers;
using ControllerCommon.Services;

namespace VLSUScheduleAPIs
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
            services.AddDbContext<Services.DatabaseContext>(x =>
            {
                x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "VLSUScheduleAPIs",
                    Description = "VLSUScheduleAPIs"
                });
            });

            var authSettings = Configuration.GetSection("AuthSettings").Get<AuthorizeSettings>();
            services.AddSingleton(authSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidAudience = authSettings.Audience,
                    ValidIssuer = authSettings.Issuer,
                    IssuerSigningKey = authSettings.SecurityKey
                };
            });

            services.AddTransient<IConsulWrapper, ConsulWrapper>();

            var consulSettings = Configuration.GetSection("consulConfig").Get<ConsulSettings>();
            services.AddSingleton(consulSettings);
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(consulSettings.Address);
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VLSUScheduleAPIs");
            });

            app.RegisterWithConsul(applicationLifetime);
            app.UseMvc();
        }
    }
}
