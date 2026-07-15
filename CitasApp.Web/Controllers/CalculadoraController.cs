using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Web.Controllers
{
    [Authorize]
    public class CalculadoraController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
