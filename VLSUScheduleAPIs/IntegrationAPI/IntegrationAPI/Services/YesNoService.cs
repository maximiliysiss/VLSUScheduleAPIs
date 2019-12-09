using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationAPI.Services
{
    public class YesNoService
    {
        public static Dictionary<string, bool> YesNo = new Dictionary<string, bool>
        {
            { "Да", true },
            { "Нет", false }
        };
    }
}
