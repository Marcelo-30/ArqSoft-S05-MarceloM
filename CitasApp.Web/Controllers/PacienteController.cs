using CitasApp.Application.Services;
using CitasApp.Application.Security;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Web.Controllers
{
    [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
    public class PacienteController : Controller
    {
        private readonly PacienteService _pacienteService;

        public PacienteController(PacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            IReadOnlyList<Paciente> pacientes = await _pacienteService.ObtenerTodosAsync(cancellationToken);
            return View(pacientes);
        }

        public async Task<IActionResult> Detalle(string id, CancellationToken cancellationToken)
        {
            Paciente? paciente = await _pacienteService.ObtenerPorIdAsync(id, cancellationToken);
            return paciente is null ? NotFound() : View(paciente);
        }

        [HttpGet]
        public IActionResult Crear() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Paciente paciente, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(paciente);
            }

            await _pacienteService.CrearAsync(paciente, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(string id, CancellationToken cancellationToken)
        {
            Paciente? paciente = await _pacienteService.ObtenerPorIdAsync(id, cancellationToken);
            return paciente is null ? NotFound() : View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Paciente paciente, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(paciente);
            }

            bool actualizado = await _pacienteService.ActualizarAsync(paciente, cancellationToken);
            return actualizado ? RedirectToAction(nameof(Index)) : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(string id, CancellationToken cancellationToken)
        {
            bool eliminado = await _pacienteService.EliminarAsync(id, cancellationToken);
            return eliminado ? RedirectToAction(nameof(Index)) : NotFound();
        }
    }
}
