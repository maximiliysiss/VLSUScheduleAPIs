using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonLibrary.Controllers
{
    public class StringUtils
    {
        public static bool IsNullOrEmpty(params string[] strs) => strs.All(x => string.IsNullOrEmpty(x));
    }
}
