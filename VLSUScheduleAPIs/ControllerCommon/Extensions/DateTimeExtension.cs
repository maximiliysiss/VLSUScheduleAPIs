using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonLibrary.Extensions
{
    public static class DateTimeExtension
    {
        public static bool IsOdd(this DateTime date)
        {
            var startYearDate = new DateTime(date.Year, 9, 1);
            if (date < startYearDate)
                startYearDate = new DateTime(date.Year - 1, 9, 1);
            if (startYearDate.DayOfWeek.In(DayOfWeek.Saturday, DayOfWeek.Sunday))
                startYearDate = startYearDate.AddDays(8 - startYearDate.DayOfWeek.ToInt());
            var offset = (date - startYearDate).TotalDays - (8 - startYearDate.DayOfWeek.ToInt());
            return offset < 0 ? true : ((int)(offset / 7) % 2) == 1;
        }
    }
}
