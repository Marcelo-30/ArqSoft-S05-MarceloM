using CitasApp.Application.Exceptions;
using CitasApp.Application.Services;
using CitasApp.Application.Security;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CitasApp.Web.Controllers
{
    [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
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

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            IReadOnlyList<Cita> citas = await _citaService.ObtenerTodasAsync(cancellationToken);
            return View(citas);
        }

        public async Task<IActionResult> Detalle(string id, CancellationToken cancellationToken)
        {
            Cita? cita = await _citaService.ObtenerPorIdAsync(id, cancellationToken);
            return cita is null ? NotFound() : View(cita);
        }

        public async Task<IActionResult> PorPaciente(
            string pacienteId,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Cita> citas = await _citaService.ObtenerPorPacienteAsync(
                pacienteId,
                cancellationToken);
            return View(citas);
        }

        [HttpGet]
        public async Task<IActionResult> Crear(CancellationToken cancellationToken)
        {
            await CargarListasAsync(null, null, cancellationToken);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Cita cita, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _citaService.CrearAsync(cita, cancellationToken);
                    return RedirectToAction(nameof(Index));
                }
                catch (ValidacionCitaException exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
                catch (ConflictoHorarioCitaException exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            await CargarListasAsync(cita.PacienteId, cita.MedicoId, cancellationToken);
            return View(cita);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(string id, CancellationToken cancellationToken)
        {
            Cita? cita = await _citaService.ObtenerPorIdAsync(id, cancellationToken);
            if (cita is null)
            {
                return NotFound();
            }

            await CargarListasAsync(cita.PacienteId, cita.MedicoId, cancellationToken);
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Cita cita, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool actualizado = await _citaService.ActualizarAsync(cita, cancellationToken);
                    if (!actualizado)
                    {
                        return NotFound();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (ValidacionCitaException exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
                catch (ConflictoHorarioCitaException exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            await CargarListasAsync(cita.PacienteId, cita.MedicoId, cancellationToken);
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(string id, CancellationToken cancellationToken)
        {
            bool eliminado = await _citaService.EliminarAsync(id, cancellationToken);
            return eliminado ? RedirectToAction(nameof(Index)) : NotFound();
        }

        private async Task CargarListasAsync(
            string? pacienteSeleccionado,
            string? medicoSeleccionado,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Paciente> pacientes = await _pacienteService.ObtenerTodosAsync(cancellationToken);
            IReadOnlyList<Medico> medicos = await _medicoService.ObtenerTodosAsync(cancellationToken);

            ViewBag.Pacientes = new SelectList(
                pacientes.Select(paciente => new
                {
                    paciente.Id,
                    Nombre = $"{paciente.Nombre} {paciente.Apellido}".Trim()
                }),
                "Id",
                "Nombre",
                pacienteSeleccionado);

            ViewBag.Medicos = new SelectList(
                medicos.Select(medico => new
                {
                    medico.Id,
                    Nombre = $"{medico.Nombre} {medico.Apellido}".Trim()
                }),
                "Id",
                "Nombre",
                medicoSeleccionado);
        }
    }
}
