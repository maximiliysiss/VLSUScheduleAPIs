using System;
using System.Collections.Generic;
using System.Text;

namespace NetServiceConnection.NetContext
{
    public class NetContext
    {
        protected Func<string, string> generateUri;

        public virtual void OnConfiguration()
        {

        }
    }
}
