using CitasApp.Api.Dtos;
using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Route("api/medicos/{medicoId}/agenda")]
    public class AgendaMedicoController : ControllerBase
    {
        private readonly CitaService _citaService;
        private readonly MedicoService _medicoService;
        private readonly PacienteService _pacienteService;

        public AgendaMedicoController(
            CitaService citaService,
            MedicoService medicoService,
            PacienteService pacienteService)
        {
            _citaService = citaService;
            _medicoService = medicoService;
            _pacienteService = pacienteService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AgendaMedicoDto>>> ObtenerAgenda(
            string medicoId,
            CancellationToken cancellationToken)
        {
            Medico? medico = await _medicoService.ObtenerPorIdAsync(medicoId, cancellationToken);

            if (medico == null)
            {
                return NotFound(new { mensaje = "No se encontró el médico solicitado." });
            }

            List<AgendaMedicoDto> agenda = (await ObtenerAgendaMedicoAsync(medico, cancellationToken))
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Hora)
                .ToList();

            return Ok(agenda);
        }

        [HttpGet("hoy")]
        public async Task<ActionResult<IReadOnlyList<AgendaMedicoDto>>> ObtenerAgendaDeHoy(
            string medicoId,
            CancellationToken cancellationToken)
        {
            Medico? medico = await _medicoService.ObtenerPorIdAsync(medicoId, cancellationToken);

            if (medico == null)
            {
                return NotFound(new { mensaje = "No se encontró el médico solicitado." });
            }

            DateOnly hoy = DateOnly.FromDateTime(DateTime.Today);
            List<AgendaMedicoDto> agenda = (await ObtenerAgendaMedicoAsync(medico, cancellationToken))
                .Where(c => c.Fecha == hoy)
                .OrderBy(c => c.Hora)
                .ToList();

            return Ok(agenda);
        }

        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<IReadOnlyList<AgendaMedicoDto>>> ObtenerAgendaPorFecha(
            string medicoId,
            DateOnly fecha,
            CancellationToken cancellationToken)
        {
            Medico? medico = await _medicoService.ObtenerPorIdAsync(medicoId, cancellationToken);

            if (medico == null)
            {
                return NotFound(new { mensaje = "No se encontró el médico solicitado." });
            }

            List<AgendaMedicoDto> agenda = (await ObtenerAgendaMedicoAsync(medico, cancellationToken))
                .Where(c => c.Fecha == fecha)
                .OrderBy(c => c.Hora)
                .ToList();

            return Ok(agenda);
        }

        private async Task<IEnumerable<AgendaMedicoDto>> ObtenerAgendaMedicoAsync(
            Medico medico,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Paciente> pacientes = await _pacienteService.ObtenerTodosAsync(cancellationToken);
            Dictionary<string, Paciente> pacientesPorId = pacientes.ToDictionary(paciente => paciente.Id);
            IReadOnlyList<Cita> citas = await _citaService.ObtenerPorMedicoAsync(
                medico.Id,
                cancellationToken);

            return citas
                .Select(c =>
                {
                    pacientesPorId.TryGetValue(c.PacienteId, out Paciente? paciente);

                    return new AgendaMedicoDto
                    {
                        CitaId = c.Id,
                        Fecha = c.Fecha,
                        Hora = c.Hora,
                        Estado = c.Estado,
                        Motivo = c.Motivo,
                        MedicoId = medico.Id,
                        MedicoNombre = $"{medico.Nombre} {medico.Apellido}".Trim(),
                        PacienteId = c.PacienteId,
                        PacienteNombre = paciente == null
                            ? "Paciente no encontrado"
                            : $"{paciente.Nombre} {paciente.Apellido}".Trim(),
                        PacienteTelefono = paciente?.Telefono ?? string.Empty
                    };
                });
        }
    }
}
