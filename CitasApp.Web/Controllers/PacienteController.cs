using Microsoft.AspNetCore.Mvc;
using CitasApp.Application.Services;
using CitasApp.Domain.Models;

namespace CitasApp.Web.Controllers
{
    public class PacienteController : Controller
    {
        private readonly PacienteService _pacienteService;

        public PacienteController(PacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        public IActionResult Index()
        {
            var pacientes = _pacienteService.ObtenerTodos();
            return View(pacientes);
        }

        public IActionResult Detalle(string id)
        {
            var paciente = _pacienteService.ObtenerPorId(id);

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
            _pacienteService.Crear(paciente);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(string id)
        {
            var paciente = _pacienteService.ObtenerPorId(id);

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
            var actualizado = _pacienteService.Actualizar(paciente);

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
            var eliminado = _pacienteService.Eliminar(id);

            if (!eliminado)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }
    }
}
