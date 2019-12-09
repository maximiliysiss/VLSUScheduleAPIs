using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationAPI.Services
{
    public class RusDayOfWeekService
    {
        private static Dictionary<string, DayOfWeek> rusDayOfWeeks = new Dictionary<string, DayOfWeek> {
            { "Понедельник", DayOfWeek.Monday},
            { "Вторник", DayOfWeek.Tuesday},
            { "Среда", DayOfWeek.Wednesday},
            { "Четверг", DayOfWeek.Thursday},
            { "Пятница", DayOfWeek.Friday},
            { "Суббота", DayOfWeek.Saturday},
            { "Воскресение", DayOfWeek.Sunday},
        };

        public DayOfWeek this[string index] => rusDayOfWeeks[index];
    }
}
