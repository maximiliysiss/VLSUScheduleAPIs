using System;
using System.Collections.Generic;
using System.Text;

namespace Commonlibrary.Models
{
    public class Teacher: User
    {
        public List<IllCard> IllCards { get; set; }
    }
}
