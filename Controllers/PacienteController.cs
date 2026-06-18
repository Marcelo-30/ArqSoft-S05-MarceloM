using Microsoft.AspNetCore.Mvc;
using CitasApp.Models;
using CitasApp.Services;

namespace CitasApp.Controllers
{
    public class PacienteController : Controller
    {
        private readonly IJsonFileService<Paciente> _pacienteService;
        private readonly IJsonFileService<Cita> _citaService;

        public PacienteController(
            IJsonFileService<Paciente> pacienteService,
            IJsonFileService<Cita> citaService)
        {
            _pacienteService = pacienteService;
            _citaService = citaService;
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
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Paciente paciente)
        {
            var pacientes = _pacienteService.Leer();

            paciente.Id = GenerarSiguienteId(pacientes);
            pacientes.Add(paciente);

            _pacienteService.Guardar(pacientes);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(string id)
        {
            var pacientes = _pacienteService.Leer();
            var paciente = pacientes.FirstOrDefault(p => p.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Paciente paciente)
        {
            var pacientes = _pacienteService.Leer();
            var pacienteExistente = pacientes.FirstOrDefault(p => p.Id == paciente.Id);

            if (pacienteExistente == null)
            {
                return NotFound();
            }

            pacienteExistente.Nombre = paciente.Nombre;
            pacienteExistente.Apellido = paciente.Apellido;
            pacienteExistente.Email = paciente.Email;
            pacienteExistente.Telefono = paciente.Telefono;

            _pacienteService.Guardar(pacientes);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(string id)
        {
            var pacientes = _pacienteService.Leer();
            var paciente = pacientes.FirstOrDefault(p => p.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            pacientes.Remove(paciente);
            _pacienteService.Guardar(pacientes);

            var citas = _citaService.Leer();
            var citasActualizadas = citas
                .Where(c => c.PacienteId != id)
                .ToList();

            _citaService.Guardar(citasActualizadas);

            return RedirectToAction("Index");
        }

        private static string GenerarSiguienteId(List<Paciente> pacientes)
        {
            var ultimoNumero = pacientes
                .Select(p => p.Id)
                .Where(id => !string.IsNullOrWhiteSpace(id) && id.StartsWith("P"))
                .Select(id => int.TryParse(id[1..], out var numero) ? numero : 0)
                .DefaultIfEmpty(0)
                .Max();

            return $"P{ultimoNumero + 1}";
        }
    }
}
