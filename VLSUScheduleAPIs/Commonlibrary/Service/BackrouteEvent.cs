using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commonlibrary.Service
{
    public class BackrouteEvent
    {
        public string Name { get; set; }
    }

    public class BackrouteEventWithArg<T>
    {
        public string Name { get; set; }
        public T Arg { get; set; }
    }
}
