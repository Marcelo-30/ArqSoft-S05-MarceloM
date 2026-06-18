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
        public ActionResult<List<AgendaMedicoDto>> ObtenerAgenda(string medicoId)
        {
            var medico = _medicoService.ObtenerPorId(medicoId);

            if (medico == null)
            {
                return NotFound(new { mensaje = "No se encontró el médico solicitado." });
            }

            var agenda = ObtenerAgendaMedico(medico)
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Hora)
                .ToList();

            return Ok(agenda);
        }

        [HttpGet("hoy")]
        public ActionResult<List<AgendaMedicoDto>> ObtenerAgendaDeHoy(string medicoId)
        {
            var medico = _medicoService.ObtenerPorId(medicoId);

            if (medico == null)
            {
                return NotFound(new { mensaje = "No se encontró el médico solicitado." });
            }

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var agenda = ObtenerAgendaMedico(medico)
                .Where(c => c.Fecha == hoy)
                .OrderBy(c => c.Hora)
                .ToList();

            return Ok(agenda);
        }

        [HttpGet("fecha/{fecha}")]
        public ActionResult<List<AgendaMedicoDto>> ObtenerAgendaPorFecha(string medicoId, DateOnly fecha)
        {
            var medico = _medicoService.ObtenerPorId(medicoId);

            if (medico == null)
            {
                return NotFound(new { mensaje = "No se encontró el médico solicitado." });
            }

            var agenda = ObtenerAgendaMedico(medico)
                .Where(c => c.Fecha == fecha)
                .OrderBy(c => c.Hora)
                .ToList();

            return Ok(agenda);
        }

        private IEnumerable<AgendaMedicoDto> ObtenerAgendaMedico(Medico medico)
        {
            var pacientes = _pacienteService.ObtenerTodos();

            return _citaService.ObtenerTodas()
                .Where(c => c.MedicoId == medico.Id)
                .Select(c =>
                {
                    var paciente = pacientes.FirstOrDefault(p => p.Id == c.PacienteId);

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
