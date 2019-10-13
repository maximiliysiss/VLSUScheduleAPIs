using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Text;

namespace Commonlibrary.Models
{
    public enum UserType
    {
        Teacher,
        Student,
        Integration
    }

    public class User
    {
        public int ID { get; set; }
        public string FIO { get; set; }
        public DateTime Birthday { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public string Token { get; set; }

        [NotMapped]
        public ClaimsIdentity ClaimsIdentity => new ClaimsIdentity(new Claim[] {
            new Claim(ClaimsIdentity.DefaultRoleClaimType, UserType.ToString()),
            new Claim(ClaimsIdentity.DefaultNameClaimType, Login)
        });
    }
}
