using System;
using System.Collections.Generic;
using System.Text;

namespace Commonlibrary.Models
{
    public class Schedule
    {
        public int ID { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int SubGroup { get; set; }
        public DateTime Time { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}
