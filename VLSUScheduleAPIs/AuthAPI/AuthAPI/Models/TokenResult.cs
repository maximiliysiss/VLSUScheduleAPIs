using Commonlibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAPI.Models
{
    public class TokenResult
    {
        public string JWTRefresh { get; set; }
        public string JWTAccess { get; set; }
        public UserType UserType { get; set; }
    }
}
