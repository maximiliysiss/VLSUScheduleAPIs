using System;

namespace Commonlibrary.Models
{
    public class IllCard
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}