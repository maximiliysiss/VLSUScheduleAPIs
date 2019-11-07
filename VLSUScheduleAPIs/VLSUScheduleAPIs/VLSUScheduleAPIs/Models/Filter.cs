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
        /// <summary>
        /// 0 - Default
        /// 1 - Teacher
        /// 2 - Group
        /// 3 - Auditory
        /// </summary>
        public FilterType FilterType { get; set; }
    }
}
