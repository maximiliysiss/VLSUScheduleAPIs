using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonLibrary.Extensions
{
    public static class ObjectExtension
    {
        public static bool In<T>(this T obj, params T[] param) => param.Contains(obj);
    }

    public static class DayOfWeekExtension
    {
        public static int ToInt(this DayOfWeek dayOfWeek) => dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
    }
}
