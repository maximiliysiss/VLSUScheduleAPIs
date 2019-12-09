using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Commonlibrary.Models
{
    public class Schedule
    {
        public int ID { get; set; }
        public int AuditoryId { get; set; }
        public virtual Auditory Auditory { get; set; }
        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }
        public int TeacherId { get; set; }
        [NotMapped]
        public Teacher Teacher { get; set; }
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        public int SubGroup { get; set; }
        public DateTime Time { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool Odd { get; set; }
    }
}
