using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IntegrationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        ILogger logger;

        public ImportController(ILogger<ImportController> logger)
        {
            this.logger = logger;
        }

        [HttpPost("excel")]
        public ActionResult UploadScheduleExcel(IFormFile file)
        {
            if (file.Length == 0)
                return BadRequest();
            logger.LogInformation("Start loading excel document");

            try
            {
                using (var workBook = new XLWorkbook(file.OpenReadStream()))
                {

                }
            }
            catch (Exception)
            {
                return BadRequest();
            }

            logger.LogInformation("End loading excel document");

            return Ok();
        }

        [HttpPost("json")]
        public ActionResult UploadScheduleJson()
        {
            return Ok();
        }

        [HttpPost("xml")]
        public ActionResult UploadScheduleXML()
        {
            return Ok();
        }
    }
}