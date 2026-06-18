using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CitasApp.Application.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Web.Controllers
{
    public class CitaController : Controller
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IMedicoRepository _medicoRepository;

        public CitaController(
            ICitaRepository citaRepository,
            IPacienteRepository pacienteRepository,
            IMedicoRepository medicoRepository)
        {
            _citaRepository = citaRepository;
            _pacienteRepository = pacienteRepository;
            _medicoRepository = medicoRepository;
        }

        public IActionResult Index()
        {
            var citas = _citaRepository.Leer();
            return View(citas);
        }

        public IActionResult Detalle(string id)
        {
            var citas = _citaRepository.Leer();
            var cita = citas.FirstOrDefault(c => c.Id == id);

            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        public IActionResult PorPaciente(string pacienteId)
        {
            var citas = _citaRepository.Leer();

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
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Cita cita)
        {
            var citas = _citaRepository.Leer();

            cita.Id = GenerarSiguienteId(citas);

            if (string.IsNullOrWhiteSpace(cita.Estado))
            {
                cita.Estado = "Pendiente";
            }

            citas.Add(cita);

            _citaRepository.Guardar(citas);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(string id)
        {
            var citas = _citaRepository.Leer();
            var cita = citas.FirstOrDefault(c => c.Id == id);

            if (cita == null)
            {
                return NotFound();
            }

            CargarListas(cita.PacienteId, cita.MedicoId);
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Cita cita)
        {
            var citas = _citaRepository.Leer();
            var citaExistente = citas.FirstOrDefault(c => c.Id == cita.Id);

            if (citaExistente == null)
            {
                return NotFound();
            }

            citaExistente.PacienteId = cita.PacienteId;
            citaExistente.MedicoId = cita.MedicoId;
            citaExistente.Fecha = cita.Fecha;
            citaExistente.Hora = cita.Hora;
            citaExistente.Motivo = cita.Motivo;
            citaExistente.Estado = string.IsNullOrWhiteSpace(cita.Estado)
                ? "Pendiente"
                : cita.Estado;

            _citaRepository.Guardar(citas);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(string id)
        {
            var citas = _citaRepository.Leer();
            var cita = citas.FirstOrDefault(c => c.Id == id);

            if (cita == null)
            {
                return NotFound();
            }

            citas.Remove(cita);
            _citaRepository.Guardar(citas);

            return RedirectToAction("Index");
        }

        private void CargarListas(string? pacienteSeleccionado = null, string? medicoSeleccionado = null)
        {
            var pacientes = _pacienteRepository.Leer();
            var medicos = _medicoRepository.Leer();

            ViewBag.Pacientes = new SelectList(
                pacientes,
                "Id",
                "Nombre",
                pacienteSeleccionado
            );

            ViewBag.Medicos = new SelectList(
                medicos,
                "Id",
                "Nombre",
                medicoSeleccionado
            );
        }

        private static string GenerarSiguienteId(List<Cita> citas)
        {
            var ultimoNumero = citas
                .Select(c => c.Id)
                .Where(id => !string.IsNullOrWhiteSpace(id) && id.StartsWith("C"))
                .Select(id => int.TryParse(id[1..], out var numero) ? numero : 0)
                .DefaultIfEmpty(0)
                .Max();

            return $"C{ultimoNumero + 1}";
        }
    }
}
