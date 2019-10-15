using System;
using System.Collections.Generic;
using System.Text;

namespace Commonlibrary.Services.Settings
{
    public class ConsulSettings
    {
        public string Adress { get; set; }
        public string ServiceName { get; set; }
        public string ServiceId { get; set; }
        public string[] Tags { get; set; }
    }
}
