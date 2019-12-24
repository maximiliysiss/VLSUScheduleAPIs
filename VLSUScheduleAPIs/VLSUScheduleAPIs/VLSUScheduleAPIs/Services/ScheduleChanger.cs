using Commonlibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VLSUScheduleAPIs.Services
{
    public enum ReloadType
    {
        Change,
        Delete
    }


    public class ScheduleChanger
    {
        private readonly RedisService redisService;
        public readonly string name;
        public readonly string illName;
        private readonly AuthNetContext authNetContext;
        private readonly ILogger<ScheduleChanger> logger;
        private readonly IServiceScopeFactory services;
        private readonly IMessageSender messageSender;

        public ScheduleChanger(RedisService redisService, IConfiguration configuration, IServiceScopeFactory services,
            IMessageSender messageSender, AuthNetContext authNetContext, ILogger<ScheduleChanger> logger)
        {
            this.redisService = redisService;
            this.name = $"{configuration["Redis:Name"]}:";
            this.illName = $"{this.name}ill:";
            this.messageSender = messageSender;
            this.authNetContext = authNetContext;
            this.logger = logger;
            this.services = services;
        }

        public List<Schedule> Create()
        {
            using (var scope = services.CreateScope())
            {
                DatabaseContext databaseContext = scope.ServiceProvider.GetService<DatabaseContext>();
                var ills = databaseContext.IllCards.Where(x => x.Date.Date >= DateTime.Today && !x.IsDelete).ToList();
                var data = databaseContext.Schedules.ToList();
                logger.LogInformation($"Load {data.Count} schedules");
                foreach (var sh in data)
                {
                    logger.LogInformation($"Load teacher {sh.TeacherId}");
                    sh.Teacher = authNetContext.Teachers.Get(sh.TeacherId);
                }
                foreach (var sh in data)
                    redisService.SetObject($"{name}{sh.ID}:{(int)sh.DayOfWeek}:{sh.GroupId}:{sh.Odd}", sh);
                foreach (var ill in ills)
                    redisService.SetObject($"{illName}{ill.ID}", ill, (ill.Date.Date.AddDays(1) - DateTime.Now));

                var illsID = ills.Select(x => x.ScheduleId).ToList();
                return data?.Where(x => !(illsID?.Contains(x.ID) ?? false)).ToList();
            }
        }

        public List<Schedule> Load(string filter = null)
        {
            var res = redisService.GetObjectByKeysSet<Schedule>(filter ?? $"{name}*");
            var ills = redisService.GetObjectByKeysSet<IllCard>($"{illName}*")?.Select(x => x.ScheduleId).ToList();
            return res?.Where(x => !(ills?.Contains(x.ID) ?? false)).ToList();
        }

        public void Reload<T>(string header, T obj, ReloadType reloadType = ReloadType.Change) where T : class
        {
            int id = obj is Schedule sh ? sh.ID : ((IllCard)(object)obj).ID;
            string preKey = $"{(obj is Schedule ? name : illName)}{id}";
            if (reloadType == ReloadType.Delete)
                redisService.Delete(preKey);
            else
            {
                if (obj is Schedule schedule)
                {
                    if (schedule.Teacher == null)
                        schedule.Teacher = authNetContext.Teachers.Get(schedule.TeacherId);
                    redisService.SetObject(preKey, obj);
                }
                else
                    redisService.SetObject(preKey, obj, ((obj as IllCard).Date.Date.AddDays(1) - DateTime.Now));
            }
            messageSender.SendMessage(header, obj);
        }
    }
}
