using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commonlibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VLSUScheduleAPIs.Models;
using VLSUScheduleAPIs.Services;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VLSUController : ControllerBase
    {
        public DatabaseContext context;
        public RedisService redisService;

        public VLSUController(DatabaseContext context, RedisService redisService)
        {
            this.context = context;
            this.redisService = redisService;
        }

        [HttpPost]
        public ActionResult<List<Schedule>> GetSchedules(Filter[] filters = null)
        {
            var scheduleList = redisService.GetObject<List<Schedule>>("vlsu:schedule:current");
            if (scheduleList == null)
                scheduleList = InitSchedule();
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    switch (filter.FilterType)
                    {
                        case FilterType.Teacher:
                            scheduleList = scheduleList.Where(x => x.Teacher.FIO.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)).ToList();
                            break;
                        case FilterType.Group:
                            scheduleList = scheduleList.Where(x => x.Group.Name.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)).ToList();
                            break;
                        case FilterType.Auditory:
                            scheduleList = scheduleList.Where(x => x.Auditory.Name.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)).ToList();
                            break;
                        case FilterType.Default:
                            scheduleList = scheduleList.Where(x => x.Auditory.Name.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)
                                                                || x.Group.Name.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)
                                                                || x.Teacher.FIO.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)).ToList();
                            break;
                    }
                }
            }
            return scheduleList;
        }

        private List<Schedule> InitSchedule()
        {
            var schedule = context.Schedules.ToList();
            redisService.SetObject("vlsu:schedule:current", schedule);
            return schedule;
        }
    }
}