using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationAPI.Services
{
    public class RusDayOfWeekService
    {
        private static Dictionary<string, DayOfWeek> rusDayOfWeeks = new Dictionary<string, DayOfWeek> {
            { "понедельник", DayOfWeek.Monday},
            { "вторник", DayOfWeek.Tuesday},
            { "среда", DayOfWeek.Wednesday},
            { "четверг", DayOfWeek.Thursday},
            { "пятница", DayOfWeek.Friday},
            { "суббота", DayOfWeek.Saturday},
            { "воскресение", DayOfWeek.Sunday},
        };

        public DayOfWeek this[string index] => rusDayOfWeeks[index.ToLower()];
    }
}
