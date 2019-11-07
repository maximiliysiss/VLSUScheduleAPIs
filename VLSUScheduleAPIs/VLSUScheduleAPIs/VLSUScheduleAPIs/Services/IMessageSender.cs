using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLSUScheduleAPIs.Services
{
    public interface IMessageSender
    {
        void Init();
        void SendMessage(string header, object obj);
    }
}
