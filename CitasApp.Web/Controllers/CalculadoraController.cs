using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Web.Controllers
{
    public class CalculadoraController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
