using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Models
{
    public class Teacher: User
    {
        public List<IllCard> IllCards { get; set; }
    }
}
