using System;

namespace Commonlibrary.Models
{
    public class IllCard: IModel
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public Schedule Schedule { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}