using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CitasApp.Application.Services;
using CitasApp.Domain.Models;

namespace CitasApp.Web.Controllers
{
    public class CitaController : Controller
    {
        private readonly CitaService _citaService;
        private readonly PacienteService _pacienteService;
        private readonly MedicoService _medicoService;

        public CitaController(
            CitaService citaService,
            PacienteService pacienteService,
            MedicoService medicoService)
        {
            _citaService = citaService;
            _pacienteService = pacienteService;
            _medicoService = medicoService;
        }

        public IActionResult Index()
        {
            var citas = _citaService.ObtenerTodas();
            return View(citas);
        }

        public IActionResult Detalle(string id)
        {
            var cita = _citaService.ObtenerPorId(id);

            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        public IActionResult PorPaciente(string pacienteId)
        {
            var citasPaciente = _citaService.ObtenerPorPaciente(pacienteId);
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
            _citaService.Crear(cita);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(string id)
        {
            var cita = _citaService.ObtenerPorId(id);

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
            var actualizado = _citaService.Actualizar(cita);

            if (!actualizado)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(string id)
        {
            var eliminado = _citaService.Eliminar(id);

            if (!eliminado)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        private void CargarListas(string? pacienteSeleccionado = null, string? medicoSeleccionado = null)
        {
            var pacientes = _pacienteService.ObtenerTodos();
            var medicos = _medicoService.ObtenerTodos();

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
    }
}
