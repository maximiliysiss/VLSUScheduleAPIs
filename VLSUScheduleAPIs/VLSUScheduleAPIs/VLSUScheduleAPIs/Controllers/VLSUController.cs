using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Commonlibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VLSUScheduleAPIs.Models;
using VLSUScheduleAPIs.Services;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VLSUController : ControllerBase
    {
        private readonly AuthNetContext authNetContext;
        private readonly DatabaseContext context;
        private readonly RedisService redisService;

        public VLSUController(DatabaseContext context, RedisService redisService, AuthNetContext authNetContext)
        {
            this.context = context;
            this.redisService = redisService;
            this.authNetContext = authNetContext;
        }

        [HttpGet]
        public ActionResult<List<Schedule>> GetSchedules()
        {
            var scheduleList = redisService.GetObject<List<Schedule>>("vlsu:schedule:current");
            if (scheduleList == null)
                scheduleList = InitSchedule();
            return scheduleList;
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult<List<Schedule>> GetStudentSchedule()
        {
            if (!int.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value, out var id))
                return NotFound();
            var user = authNetContext.Students.Get(id);
            return redisService.GetObject<List<Schedule>>("vlsu:schedule:current").Where(x => x.GroupId == user.GroupId).ToList();
        }

        [HttpPost]
        public ActionResult<List<Schedule>> GetSchedules([FromBody]Filter[] filters = null)
        {
            var scheduleList = redisService.GetObject<List<Schedule>>("vlsu:schedule:current");
            if (scheduleList == null)
                scheduleList = InitSchedule();
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