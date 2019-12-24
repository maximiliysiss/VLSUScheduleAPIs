using System;
using System.Collections.Generic;
using System.Linq;
using Commonlibrary.Models;
using CommonLibrary.Extensions;
using Microsoft.AspNetCore.Mvc;
using VLSUScheduleAPIs.Models;
using VLSUScheduleAPIs.Services;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VLSUController : ControllerBase
    {
        private List<Schedule> Schedules(string filter = null) => scheduleChanger.Load(filter);

        private readonly AuthNetContext authNetContext;
        private readonly DatabaseContext context;
        private readonly ScheduleChanger scheduleChanger;

        public VLSUController(AuthNetContext authNetContext, DatabaseContext context, ScheduleChanger scheduleChanger)
        {
            this.authNetContext = authNetContext;
            this.context = context;
            this.scheduleChanger = scheduleChanger;
        }

        [HttpGet]
        public List<Schedule> GetScheduleByDate(int @class, int year, int month, int day)
        {
            var date = new DateTime(year, month, day);
            return Schedules($"{scheduleChanger.name}*:{(int)date.DayOfWeek}:{@class}:{date.IsOdd()}:*").OrderBy(x => x.Time.TimeOfDay).ToList();
        }

        [HttpGet]
        public List<FilterResult> Filter(string filter)
        {
            List<FilterResult> filterResults = new List<FilterResult>();
            filterResults.AddRange(context.Groups.Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .Select(x => new FilterResult { FilterType = FilterType.Group, ID = x.ID, Value = x.Name }));
            filterResults.AddRange(authNetContext.Teachers.Where(x => (x.FIO?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false) || (x.ShortName?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false))
                .Select(x => new FilterResult { FilterType = FilterType.Teacher, ID = x.ID, Value = x.FIO }));
            filterResults.AddRange(context.Lessons.Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .Select(x => new FilterResult { FilterType = FilterType.Lesson, ID = x.ID, Value = x.Name }));
            return filterResults;
        }

        [HttpPost("{year}/{month}/{day}")]
        public List<Schedule> Filter(FilterResult filter, int year, int month, int day)
        {
            var date = new DateTime(year, month, day);
            switch (filter.FilterType)
            {
                case FilterType.Teacher:
                    return Schedules($"{scheduleChanger.name}*:{(int)date.DayOfWeek}:*:{date.IsOdd()}:{filter.ID}:*").OrderBy(x => x.Time.TimeOfDay).ToList();
                case FilterType.Group:
                    return Schedules($"{scheduleChanger.name}*:{(int)date.DayOfWeek}:{filter.ID}:{date.IsOdd()}:*").OrderBy(x => x.Time.TimeOfDay).ToList();
                case FilterType.Lesson:
                    return Schedules($"{scheduleChanger.name}*:{(int)date.DayOfWeek}:*:{date.IsOdd()}:*:{filter.ID}").OrderBy(x => x.Time.TimeOfDay).ToList();
            }
            return null;
        }

        [HttpPost]
        [ActionName("Filter")]
        public List<Schedule> FilterFull(FilterResult filter)
        {
            switch (filter.FilterType)
            {
                case FilterType.Teacher:
                    return Schedules($"{scheduleChanger.name}*:*:*:*:{filter.ID}:*").OrderBy(x => x.Time.TimeOfDay).ToList();
                case FilterType.Group:
                    return Schedules($"{scheduleChanger.name}*:*:{filter.ID}:*:*").OrderBy(x => x.Time.TimeOfDay).ToList();
                case FilterType.Lesson:
                    return Schedules($"{scheduleChanger.name}*:*:*:*:*:{filter.ID}").OrderBy(x => x.Time.TimeOfDay).ToList();
            }
            return null;
        }
    }
}