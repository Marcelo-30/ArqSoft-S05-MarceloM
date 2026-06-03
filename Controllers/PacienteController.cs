using Microsoft.AspNetCore.Mvc;
using CitasApp.Models;
using CitasApp.Services;

namespace CitasApp.Controllers
{
    public class PacienteController : Controller
    {
        private readonly JsonFileService<Paciente> _pacienteService;

        public PacienteController(IWebHostEnvironment env)
        {
            var ruta = Path.Combine(env.ContentRootPath, "Data", "pacientes.json");
            _pacienteService = new JsonFileService<Paciente>(ruta);
        }

        public IActionResult Index()
        {
            var pacientes = _pacienteService.Leer();
            return View(pacientes);
        }

        public IActionResult Detalle(string id)
        {
            var pacientes = _pacienteService.Leer();
            var paciente = pacientes.FirstOrDefault(p => p.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Paciente paciente)
        {
            var pacientes = _pacienteService.Leer();

            paciente.Id = "P" + (pacientes.Count + 1);

            pacientes.Add(paciente);

            _pacienteService.Guardar(pacientes);

            return RedirectToAction("Index");
        }
    }
}