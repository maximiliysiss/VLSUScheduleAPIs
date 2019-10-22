using System;
using System.Collections.Generic;
using System.Text;

namespace Commonlibrary.Services.Settings
{
    public class ConsulSettings
    {
        public string Address { get; set; }
        public string ServiceName { get; set; }
        public string ServiceId { get; set; }
        public List<string> Tags { get; set; }
    }
}
