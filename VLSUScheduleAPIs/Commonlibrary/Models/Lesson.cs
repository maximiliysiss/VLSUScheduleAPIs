﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Commonlibrary.Models
{
    public class Lesson: IModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
