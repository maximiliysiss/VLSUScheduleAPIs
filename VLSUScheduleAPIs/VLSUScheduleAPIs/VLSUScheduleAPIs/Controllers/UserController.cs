using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commonlibrary.Controllers;
using Commonlibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VLSUScheduleAPIs.Services;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AuthNetContext authNetContext;

        public UserController(AuthNetContext authNetContext)
        {
            this.authNetContext = authNetContext;
        }

        [Authorize]
        [HttpGet]
        public User GetUser() => authNetContext.Users.Get(this.UserId() ?? 0);

        [Authorize]
        [HttpGet]
        public Student GetStudent() => authNetContext.Students.Get(this.UserId() ?? 0);
    }
}