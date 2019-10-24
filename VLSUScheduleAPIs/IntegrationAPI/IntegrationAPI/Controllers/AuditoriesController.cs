using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commonlibrary.Models;
using IntegrationAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriesController : ControllerBase
    {
        VlsuContext vlsuContext;

        public AuditoriesController(VlsuContext vlsuContext)
        {
            this.vlsuContext = vlsuContext;
        }

        [HttpPost]
        public ActionResult Add(Auditory auditory)
        {
            vlsuContext.Auditories.Add(auditory);
            return Ok();
        }
    }
}