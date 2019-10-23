using Commonlibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLSUScheduleAPIs.Models
{
    public enum FilterType
    {
        Default,
        Teacher,
        Group,
        Auditory
    }

    public class Filter
    {
        public string Value { get; set; }
        public FilterType FilterType { get; set; }
    }
}
