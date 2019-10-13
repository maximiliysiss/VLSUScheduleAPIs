using Microsoft.AspNetCore.Mvc;
using VLSUScheduleAPIs.Services;

namespace VLSUScheduleAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ScheduleController : ControllerBase
    {
        DatabaseContext databaseContext;

        public ScheduleController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
    }
}