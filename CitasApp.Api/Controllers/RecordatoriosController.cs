using CitasApp.Api.Dtos;
using CitasApp.Application.Security;
using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
    [Route("api/recordatorios")]
    public class RecordatoriosController : ControllerBase
    {
        private readonly CitaService _citaService;
        private readonly PacienteService _pacienteService;
        private readonly MedicoService _medicoService;

        public RecordatoriosController(
            CitaService citaService,
            PacienteService pacienteService,
            MedicoService medicoService)
        {
            _citaService = citaService;
            _pacienteService = pacienteService;
            _medicoService = medicoService;
        }

        [HttpGet("pendientes")]
        public async Task<ActionResult<IReadOnlyList<RecordatorioWhatsappDto>>> ObtenerRecordatoriosPendientes(
            [FromQuery] int dias = 1,
            CancellationToken cancellationToken = default)
        {
            if (dias < 0)
            {
                return BadRequest(new { mensaje = "El parámetro dias no puede ser negativo." });
            }

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var fechaLimite = hoy.AddDays(dias);

            List<RecordatorioWhatsappDto> recordatorios = (await CrearRecordatoriosAsync(cancellationToken))
                .Where(r => r.Fecha >= hoy && r.Fecha <= fechaLimite)
                .Where(r => !string.Equals(r.Estado, "Cancelada", StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.Hora)
                .ToList();

            return Ok(recordatorios);
        }

        [HttpPost("whatsapp/{citaId}")]
        public async Task<ActionResult<EnviarWhatsappResponseDto>> EnviarRecordatorioWhatsapp(
            string citaId,
            CancellationToken cancellationToken)
        {
            RecordatorioWhatsappDto? recordatorio = (await CrearRecordatoriosAsync(cancellationToken))
                .FirstOrDefault(r => r.CitaId == citaId);

            if (recordatorio == null)
            {
                return NotFound(new { mensaje = "No se encontró la cita solicitada." });
            }

            if (string.IsNullOrWhiteSpace(recordatorio.PacienteTelefono))
            {
                return BadRequest(new { mensaje = "El paciente no tiene teléfono registrado." });
            }

            var respuesta = new EnviarWhatsappResponseDto
            {
                CitaId = recordatorio.CitaId,
                Paciente = recordatorio.PacienteNombre,
                Telefono = recordatorio.PacienteTelefono,
                Mensaje = recordatorio.Mensaje,
                WhatsappUrl = recordatorio.WhatsappUrl,
                Enviado = true,
                Nota = "Simulación de envío. Para envío real se debe conectar Meta WhatsApp Cloud API o Twilio."
            };

            return Ok(respuesta);
        }

        private async Task<IEnumerable<RecordatorioWhatsappDto>> CrearRecordatoriosAsync(
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Paciente> pacientes = await _pacienteService.ObtenerTodosAsync(cancellationToken);
            IReadOnlyList<Medico> medicos = await _medicoService.ObtenerTodosAsync(cancellationToken);
            IReadOnlyList<Cita> citas = await _citaService.ObtenerTodasAsync(cancellationToken);
            Dictionary<string, Paciente> pacientesPorId = pacientes.ToDictionary(paciente => paciente.Id);
            Dictionary<string, Medico> medicosPorId = medicos.ToDictionary(medico => medico.Id);

            return citas
                .Select(cita =>
                {
                    pacientesPorId.TryGetValue(cita.PacienteId, out Paciente? paciente);
                    medicosPorId.TryGetValue(cita.MedicoId, out Medico? medico);

                    var pacienteNombre = paciente == null
                        ? "Paciente no encontrado"
                        : $"{paciente.Nombre} {paciente.Apellido}".Trim();

                    var medicoNombre = medico == null
                        ? "Médico no encontrado"
                        : $"Dr(a). {medico.Nombre} {medico.Apellido}".Trim();

                    var mensaje = $"Hola {pacienteNombre}, le recordamos su cita médica con {medicoNombre} el {cita.Fecha:dd/MM/yyyy} a las {cita.Hora:HH\\:mm}. Motivo: {cita.Motivo}.";

                    return new RecordatorioWhatsappDto
                    {
                        CitaId = cita.Id,
                        Fecha = cita.Fecha,
                        Hora = cita.Hora,
                        Estado = cita.Estado,
                        PacienteId = cita.PacienteId,
                        PacienteNombre = pacienteNombre,
                        PacienteTelefono = paciente?.Telefono ?? string.Empty,
                        MedicoId = cita.MedicoId,
                        MedicoNombre = medicoNombre,
                        Mensaje = mensaje,
                        WhatsappUrl = ConstruirWhatsappUrl(paciente?.Telefono, mensaje)
                    };
                });
        }

        private static string ConstruirWhatsappUrl(string? telefono, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(telefono))
            {
                return string.Empty;
            }

            var telefonoLimpio = new string(telefono.Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(telefonoLimpio))
            {
                return string.Empty;
            }

            return $"https://wa.me/{telefonoLimpio}?text={Uri.EscapeDataString(mensaje)}";
        }
    }
}
