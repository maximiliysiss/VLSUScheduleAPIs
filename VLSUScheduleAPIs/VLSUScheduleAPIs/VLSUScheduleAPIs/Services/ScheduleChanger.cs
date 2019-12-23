using Castle.Core.Logging;
using Commonlibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLSUScheduleAPIs.Services
{
    public class ScheduleChanger
    {
        private readonly RedisService redisService;
        private readonly string name;
        private readonly AuthNetContext authNetContext;
        private readonly ILogger<ScheduleChanger> logger;
        private readonly IServiceScopeFactory services;
        private readonly IMessageSender messageSender;

        public ScheduleChanger(RedisService redisService, IConfiguration configuration, IServiceScopeFactory services,
            IMessageSender messageSender, AuthNetContext authNetContext, ILogger<ScheduleChanger> logger)
        {
            this.redisService = redisService;
            this.name = configuration["Redis:Name"];
            this.messageSender = messageSender;
            this.authNetContext = authNetContext;
            this.logger = logger;
            this.services = services;

        }

        public List<Schedule> Create()
        {
            List<Schedule> data = new List<Schedule>();
            using (var scope = services.CreateScope())
            {
                DatabaseContext databaseContext = scope.ServiceProvider.GetService<DatabaseContext>();
                var ills = databaseContext.IllCards.Where(x => x.Date.Date >= DateTime.Today && !x.IsDelete).Select(x => x.Schedule.ID).ToArray();
                if (ills.Length != 0)
                    data = databaseContext.Schedules.FromSql("select * from Schedules where id in @p", ills).ToList();
                else
                    data = databaseContext.Schedules.ToList();
                logger.LogInformation($"Load {data.Count} schedules");
                foreach (var sh in data)
                {
                    logger.LogInformation($"Load teacher {sh.TeacherId}");
                    sh.Teacher = authNetContext.Teachers.Get(sh.TeacherId);
                }
                redisService.SetObject(name, data);
            }
            return data;
        }

        public List<Schedule> Load() => redisService.GetObject<List<Schedule>>(name);
        public void Reload<T>(string header, T obj)
        {
            Create();
            messageSender.SendMessage(header, obj);
        }
    }
}
