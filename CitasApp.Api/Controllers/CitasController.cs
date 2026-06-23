using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Route("api/citas")]
    public class CitasController : ControllerBase
    {
        private readonly CitaService _citaService;
        private readonly PacienteService _pacienteService;
        private readonly MedicoService _medicoService;

        public CitasController(
            CitaService citaService,
            PacienteService pacienteService,
            MedicoService medicoService)
        {
            _citaService = citaService;
            _pacienteService = pacienteService;
            _medicoService = medicoService;
        }

        [HttpGet]
        public ActionResult<List<Cita>> ObtenerTodas()
        {
            var citas = _citaService.ObtenerTodas();
            return Ok(citas);
        }

        [HttpGet("{id}")]
        public ActionResult<Cita> ObtenerPorId(string id)
        {
            var cita = _citaService.ObtenerPorId(id);

            if (cita == null)
            {
                return NotFound(new { mensaje = $"No se encontró la cita con id {id}." });
            }

            return Ok(cita);
        }

        [HttpGet("paciente/{pacienteId}")]
        public ActionResult<List<Cita>> ObtenerPorPaciente(string pacienteId)
        {
            var paciente = _pacienteService.ObtenerPorId(pacienteId);

            if (paciente == null)
            {
                return NotFound(new { mensaje = $"No se encontró el paciente con id {pacienteId}." });
            }

            var citas = _citaService.ObtenerPorPaciente(pacienteId);
            return Ok(citas);
        }

        [HttpPost]
        public ActionResult<Cita> Crear(Cita cita)
        {
            var error = ValidarPacienteYMedico(cita.PacienteId, cita.MedicoId);

            if (error != null)
            {
                return error;
            }

            _citaService.Crear(cita);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = cita.Id }, cita);
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(string id, Cita cita)
        {
            var error = ValidarPacienteYMedico(cita.PacienteId, cita.MedicoId);

            if (error != null)
            {
                return error;
            }

            cita.Id = id;
            var actualizado = _citaService.Actualizar(cita);

            if (!actualizado)
            {
                return NotFound(new { mensaje = $"No se encontró la cita con id {id}." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(string id)
        {
            var eliminado = _citaService.Eliminar(id);

            if (!eliminado)
            {
                return NotFound(new { mensaje = $"No se encontró la cita con id {id}." });
            }

            return NoContent();
        }

        private ActionResult? ValidarPacienteYMedico(string pacienteId, string medicoId)
        {
            if (_pacienteService.ObtenerPorId(pacienteId) == null)
            {
                return NotFound(new { mensaje = $"No se encontró el paciente con id {pacienteId}." });
            }

            if (_medicoService.ObtenerPorId(medicoId) == null)
            {
                return NotFound(new { mensaje = $"No se encontró el médico con id {medicoId}." });
            }

            return null;
        }
    }
}
