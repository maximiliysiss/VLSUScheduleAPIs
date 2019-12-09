using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Commonlibrary.Models
{
    public class IllCard
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public virtual Schedule Schedule { get; set; }
        public int TeacherId { get; set; }
        [NotMapped]
        public Teacher Teacher { get; set; }
        public bool IsDelete { get; set; } = false;
    }
}