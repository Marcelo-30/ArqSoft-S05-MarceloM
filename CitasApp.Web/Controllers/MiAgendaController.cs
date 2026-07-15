using CitasApp.Application.Exceptions;
using CitasApp.Application.Security;
using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Identity;
using CitasApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Web.Controllers
{
    [Authorize(Roles = RolesAplicacion.Medico)]
    public sealed class MiAgendaController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CitaService _citaService;
        private readonly PacienteService _pacienteService;

        public MiAgendaController(
            UserManager<ApplicationUser> userManager,
            CitaService citaService,
            PacienteService pacienteService)
        {
            _userManager = userManager;
            _citaService = citaService;
            _pacienteService = pacienteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrWhiteSpace(user?.MedicoId))
            {
                return Forbid();
            }

            IReadOnlyList<Cita> citas = await _citaService.ObtenerPorMedicoAsync(
                user.MedicoId,
                cancellationToken);
            IReadOnlyList<Paciente> pacientes = await _pacienteService.ObtenerTodosAsync(cancellationToken);
            Dictionary<string, Paciente> pacientesPorId = pacientes.ToDictionary(paciente => paciente.Id);

            List<MiAgendaItemViewModel> items = citas.Select(cita =>
            {
                pacientesPorId.TryGetValue(cita.PacienteId, out Paciente? paciente);
                string nombre = paciente is null
                    ? "Paciente no encontrado"
                    : $"{paciente.Nombre} {paciente.Apellido}".Trim();
                return new MiAgendaItemViewModel(cita, nombre);
            }).ToList();

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            string id,
            string estado,
            CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrWhiteSpace(user?.MedicoId))
            {
                return Forbid();
            }

            Cita? cita = await _citaService.ObtenerPorIdAsync(id, cancellationToken);
            if (cita is null || cita.MedicoId != user.MedicoId)
            {
                return NotFound();
            }

            string? estadoValido = EstadosCita.Todos.FirstOrDefault(
                value => value.Equals(estado, StringComparison.OrdinalIgnoreCase));
            if (estadoValido is null)
            {
                return BadRequest();
            }

            cita.Estado = estadoValido;
            try
            {
                await _citaService.ActualizarAsync(cita, cancellationToken);
            }
            catch (ConflictoHorarioCitaException exception)
            {
                TempData["Error"] = exception.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
