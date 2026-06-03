using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CitasApp.Models;
using CitasApp.Services;

namespace CitasApp.Controllers
{
    public class CitaController : Controller
    {
        private readonly JsonFileService<Cita> _citaService;
        private readonly JsonFileService<Paciente> _pacienteService;
        private readonly JsonFileService<Medico> _medicoService;

        public CitaController(IWebHostEnvironment env)
        {
            var rutaCitas = Path.Combine(env.ContentRootPath, "Data", "citas.json");
            var rutaPacientes = Path.Combine(env.ContentRootPath, "Data", "pacientes.json");
            var rutaMedicos = Path.Combine(env.ContentRootPath, "Data", "medicos.json");

            _citaService = new JsonFileService<Cita>(rutaCitas);
            _pacienteService = new JsonFileService<Paciente>(rutaPacientes);
            _medicoService = new JsonFileService<Medico>(rutaMedicos);
        }

        public IActionResult Index()
        {
            var citas = _citaService.Leer();
            return View(citas);
        }

        public IActionResult PorPaciente(string pacienteId)
        {
            var citas = _citaService.Leer();

            var citasPaciente = citas
                .Where(c => c.PacienteId == pacienteId)
                .ToList();

            return View(citasPaciente);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            CargarListas();
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Cita cita)
        {
            var citas = _citaService.Leer();

            cita.Id = "C" + (citas.Count + 1);

            if (string.IsNullOrWhiteSpace(cita.Estado))
            {
                cita.Estado = "Pendiente";
            }

            citas.Add(cita);

            _citaService.Guardar(citas);

            return RedirectToAction("Index");
        }

        private void CargarListas()
        {
            var pacientes = _pacienteService.Leer();
            var medicos = _medicoService.Leer();

            ViewBag.Pacientes = new SelectList(
                pacientes,
                "Id",
                "Nombre"
            );

            ViewBag.Medicos = new SelectList(
                medicos,
                "Id",
                "Nombre"
            );
        }
    }
}
