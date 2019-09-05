using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Models
{
    public class Schedule
    {
        public int ID { get; set; }
        public Lesson Lesson { get; set; }
        public Teacher Teacher { get; set; }
        public Group Group { get; set; }
        public int SubGroup{ get; set; }
        public DateTime Time { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsOdd { get; set; }
    }
}
