using Microsoft.AspNetCore.Mvc;
using CitasApp.Models;

namespace CitasApp.Controllers
{
    public class PacienteController : Controller
    {
        private static List<Paciente> pacientes = new List<Paciente>
        {
            new Paciente
            {
                Id = "P1",
                Nombre = "Carlos",
                Apellido = "Ramírez",
                Email = "carlos@gmail.com",
                Telefono = "9991234567"
            },
            new Paciente
            {
                Id = "P2",
                Nombre = "Ana",
                Apellido = "López",
                Email = "ana@gmail.com",
                Telefono = "9997654321"
            }
        };

        public IActionResult Index()
        {
            return View(pacientes);
        }

        public IActionResult Detalle(string id)
        {
            var paciente = pacientes.FirstOrDefault(p => p.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }
    }
}
