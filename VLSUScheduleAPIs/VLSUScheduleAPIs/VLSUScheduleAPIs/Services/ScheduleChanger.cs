using Commonlibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly DatabaseContext databaseContext;
        private readonly IMessageSender messageSender;

        public ScheduleChanger(RedisService redisService, IConfiguration configuration, DatabaseContext databaseContext,
            IMessageSender messageSender)
        {
            this.redisService = redisService;
            this.name = configuration["Redis:Name"];
            this.databaseContext = databaseContext;
            this.messageSender = messageSender;
        }

        public List<Schedule> Create()
        {
            var ills = databaseContext.IllCards.Where(x => x.Date.Date >= DateTime.Today && !x.IsDelete).Select(x => x.Schedule.ID);
            var data = databaseContext.Schedules.FromSql("select * from Schedules where id in @p", ills).ToList();
            redisService.SetObject(name, data);
            return data;
        }

        public List<Schedule> Load() => redisService.GetObject<List<Schedule>>(name);
        public void Reload<T>(string header, T obj)
        {
            Create();
            //messageSender.SendMessage(header, obj);
        }
    }
}
