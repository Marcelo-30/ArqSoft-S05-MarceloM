using Microsoft.AspNetCore.Mvc;
using CitasApp.Application.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Web.Controllers
{
    public class PacienteController : Controller
    {
        private readonly IPacienteRepository _pacienteRepository;
        private readonly ICitaRepository _citaRepository;

        public PacienteController(
            IPacienteRepository pacienteRepository,
            ICitaRepository citaRepository)
        {
            _pacienteRepository = pacienteRepository;
            _citaRepository = citaRepository;
        }

        public IActionResult Index()
        {
            var pacientes = _pacienteRepository.Leer();
            return View(pacientes);
        }

        public IActionResult Detalle(string id)
        {
            var pacientes = _pacienteRepository.Leer();
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
            var pacientes = _pacienteRepository.Leer();

            paciente.Id = GenerarSiguienteId(pacientes);
            pacientes.Add(paciente);

            _pacienteRepository.Guardar(pacientes);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(string id)
        {
            var pacientes = _pacienteRepository.Leer();
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
            var pacientes = _pacienteRepository.Leer();
            var pacienteExistente = pacientes.FirstOrDefault(p => p.Id == paciente.Id);

            if (pacienteExistente == null)
            {
                return NotFound();
            }

            pacienteExistente.Nombre = paciente.Nombre;
            pacienteExistente.Apellido = paciente.Apellido;
            pacienteExistente.Email = paciente.Email;
            pacienteExistente.Telefono = paciente.Telefono;

            _pacienteRepository.Guardar(pacientes);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(string id)
        {
            var pacientes = _pacienteRepository.Leer();
            var paciente = pacientes.FirstOrDefault(p => p.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            pacientes.Remove(paciente);
            _pacienteRepository.Guardar(pacientes);

            var citas = _citaRepository.Leer();
            var citasActualizadas = citas
                .Where(c => c.PacienteId != id)
                .ToList();

            _citaRepository.Guardar(citasActualizadas);

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
