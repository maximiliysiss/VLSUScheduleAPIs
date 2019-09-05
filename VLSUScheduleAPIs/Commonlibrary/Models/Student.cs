using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Models
{
    public class Student: User
    {
        public Group Group { get; set; }
        public int SubGroup { get; set; }
    }
}
