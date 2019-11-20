using System;
using System.Collections.Generic;
using System.Text;

namespace NetServiceConnection.NetAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LazyNetLoad : Attribute
    {
        public string Id { get; set; }
        public string Service { get; set; }
    }
}
