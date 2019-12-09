using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Commonlibrary.Models
{
    public class Teacher: User
    {
        [NotMapped]
        public List<IllCard> IllCards { get; set; }
        public string ShortName { get; set; }
    }
}
