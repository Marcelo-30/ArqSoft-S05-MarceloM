using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Web.Controllers
{
    [Authorize]
    public class MedicoController : Controller
    {
        private readonly MedicoService _medicoService;

        public MedicoController(MedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            IReadOnlyList<Medico> medicos = await _medicoService.ObtenerTodosAsync(cancellationToken);
            return View(medicos);
        }

        public async Task<IActionResult> Detalle(string id, CancellationToken cancellationToken)
        {
            Medico? medico = await _medicoService.ObtenerPorIdAsync(id, cancellationToken);
            return medico is null ? NotFound() : View(medico);
        }

        [HttpGet]
        public IActionResult Crear() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Medico medico, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(medico);
            }

            await _medicoService.CrearAsync(medico, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(string id, CancellationToken cancellationToken)
        {
            Medico? medico = await _medicoService.ObtenerPorIdAsync(id, cancellationToken);
            return medico is null ? NotFound() : View(medico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Medico medico, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(medico);
            }

            bool actualizado = await _medicoService.ActualizarAsync(medico, cancellationToken);
            return actualizado ? RedirectToAction(nameof(Index)) : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(string id, CancellationToken cancellationToken)
        {
            bool eliminado = await _medicoService.EliminarAsync(id, cancellationToken);
            return eliminado ? RedirectToAction(nameof(Index)) : NotFound();
        }
    }
}
