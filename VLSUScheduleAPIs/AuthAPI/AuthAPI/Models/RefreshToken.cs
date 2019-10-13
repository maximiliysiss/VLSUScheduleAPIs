using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAPI.Models
{
    public class RefreshToken
    {
        public string Login { get; set; }
        public string Token { get; set; }
    }
}
