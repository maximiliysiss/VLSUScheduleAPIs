using Commonlibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAPI.Models
{
    public class LoginModel
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public UserType UserType { get; set; }
    }
}
