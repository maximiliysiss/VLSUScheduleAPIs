using Microsoft.AspNetCore.Mvc;

namespace IntegrationAPI.Controllers
{
    public class ExportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}