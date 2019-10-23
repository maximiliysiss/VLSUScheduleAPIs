using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NetServiceConnection.Exception
{
    public class RollbackException : System.Exception
    {
        public RollbackException()
        {
        }

        public RollbackException(string message) : base(message)
        {
        }

        public RollbackException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected RollbackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
