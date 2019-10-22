using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetServiceConnection.NetContext
{
    public class NetContextFactory
    {
        public ConcurrentDictionary<string, NetContext> contexts = new ConcurrentDictionary<string, NetContext>();

        public NetContextFactory Configure(NetSettings netSettings)
        {
            return this;
        }

        public NetContext Get(string name) => contexts[name];
    }
}
