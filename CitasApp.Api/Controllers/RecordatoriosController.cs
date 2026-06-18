using CitasApp.Api.Dtos;
using CitasApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
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
        public ActionResult<List<RecordatorioWhatsappDto>> ObtenerRecordatoriosPendientes([FromQuery] int dias = 1)
        {
            if (dias < 0)
            {
                return BadRequest(new { mensaje = "El parámetro dias no puede ser negativo." });
            }

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var fechaLimite = hoy.AddDays(dias);

            var recordatorios = CrearRecordatorios()
                .Where(r => r.Fecha >= hoy && r.Fecha <= fechaLimite)
                .Where(r => !string.Equals(r.Estado, "Cancelada", StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.Hora)
                .ToList();

            return Ok(recordatorios);
        }

        [HttpPost("whatsapp/{citaId}")]
        public ActionResult<EnviarWhatsappResponseDto> EnviarRecordatorioWhatsapp(string citaId)
        {
            var recordatorio = CrearRecordatorios()
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

        private IEnumerable<RecordatorioWhatsappDto> CrearRecordatorios()
        {
            var pacientes = _pacienteService.ObtenerTodos();
            var medicos = _medicoService.ObtenerTodos();

            return _citaService.ObtenerTodas()
                .Select(cita =>
                {
                    var paciente = pacientes.FirstOrDefault(p => p.Id == cita.PacienteId);
                    var medico = medicos.FirstOrDefault(m => m.Id == cita.MedicoId);

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
