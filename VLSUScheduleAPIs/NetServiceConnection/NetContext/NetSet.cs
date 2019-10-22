using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetServiceConnection.NetContext
{
    public class NetSet<T>: IEnumerable<T>
    {
        private NetContext netContext;

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
