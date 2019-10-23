using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commonlibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VLSUScheduleAPIs.Models;
using VLSUScheduleAPIs.Services;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VLSUController : ControllerBase
    {
        public DatabaseContext context;

        public VLSUController(DatabaseContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult<List<Schedule>>> GetSchedules(Filter[] filters = null)
        {
            var data = context.Schedules.ToList();
        }
    }
}