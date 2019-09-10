using Microsoft.AspNetCore.Mvc;
using AuthAPI.Models;
using AuthAPI.Services;
using Commonlibrary.Models;
using System.Linq;
using System.Collections.Generic;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        DatabaseContext databaseContext;

        public AuthController(DatabaseContext database)
        {
            this.databaseContext = database;
        }

        public TokenServiceResult Login()
        {
            return new TokenServiceResult();
        }

        public TokenServiceResult Register()
        {
            return new TokenServiceResult();
        }

        public bool IsAuthorize()
        {
            return true;
        }

        public List<Student> Students()
        {
            return databaseContext.Students.ToList();
        }
    }
}