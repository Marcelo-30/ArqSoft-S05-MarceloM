using Microsoft.AspNetCore.Mvc;
using CitasApp.Models;

namespace CitasApp.Controllers
{
    public class MedicoController : Controller
    {
        private static List<Medico> medicos = new List<Medico>
        {
            new Medico
            {
                Id = "M1",
                Nombre = "Luis",
                Apellido = "Fernández",
                Especialidad = "Cardiología",
                NumeroLicencia = "LIC12345"
            },
            new Medico
            {
                Id = "M2",
                Nombre = "María",
                Apellido = "Gómez",
                Especialidad = "Pediatría",
                NumeroLicencia = "LIC67890"
            }
        };

        public IActionResult Index()
        {
            return View(medicos);
        }

        public IActionResult Detalle(string id)
        {
            var medico = medicos.FirstOrDefault(m => m.Id == id);

            if (medico == null)
            {
                return NotFound();
            }

            return View(medico);
        }
    }
}
