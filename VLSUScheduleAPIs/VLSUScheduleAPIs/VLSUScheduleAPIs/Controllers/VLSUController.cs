﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Commonlibrary.Models;
using CommonLibrary.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VLSUScheduleAPIs.Models;
using VLSUScheduleAPIs.Services;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]/[action]")]
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
        public List<Schedule> GetScheduleByDate(int @class, int year, int month, int day)
        {
            var date = new DateTime(year, month, day);
            var startYearDate = new DateTime(date.Year, 9, 1);
            if (date < startYearDate)
                startYearDate = new DateTime(date.Year - 1, 9, 1);
            if (startYearDate.DayOfWeek.In(DayOfWeek.Saturday, DayOfWeek.Sunday))
                startYearDate = startYearDate.AddDays(8 - startYearDate.DayOfWeek.ToInt());
            var offset = (date - startYearDate).TotalDays - (8 - startYearDate.DayOfWeek.ToInt());
            var isOdd = offset < 0 ? true : ((int)(offset / 7) % 2) == 1;
            var scheduleList = redisService.GetObject<List<Schedule>>("vlsu:schedule:current");
            if (scheduleList == null)
                scheduleList = InitSchedule();
            return scheduleList.Where(x => x.DayOfWeek == date.DayOfWeek && x.GroupId == @class && x.Odd == isOdd).ToList();
        }

        [HttpGet]
        public List<FilterResult> Filter(string filter)
        {
            List<FilterResult> filterResults = new List<FilterResult>();
            filterResults.AddRange(context.Groups.Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .Select(x => new FilterResult { FilterType = FilterType.Group, ID = x.ID, Value = x.Name }));
            filterResults.AddRange(authNetContext.Teachers.Where(x => x.FIO.Contains(filter, StringComparison.OrdinalIgnoreCase) || x.ShortName.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .Select(x => new FilterResult { FilterType = FilterType.Teacher, ID = x.ID, Value = x.FIO }));
            filterResults.AddRange(context.Lessons.Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .Select(x => new FilterResult { FilterType = FilterType.Lesson, ID = x.ID, Value = x.Name }));
            return filterResults;
        }

        private List<Schedule> InitSchedule()
        {
            var schedule = context.Schedules.ToList();
            foreach (var sc in schedule)
                sc.Teacher = authNetContext.Teachers.FirstOrDefault(x => x.ID == sc.TeacherId);
            redisService.SetObject("vlsu:schedule:current", schedule);
            return schedule;
        }
    }
}