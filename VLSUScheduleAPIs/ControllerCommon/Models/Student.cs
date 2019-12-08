using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Commonlibrary.Models
{
    public class Student: User
    {
        public int GroupId { get; set; }
        [NotMapped]
        public Group Group { get; set; }
        public int SubGroup { get; set; }
    }
}
