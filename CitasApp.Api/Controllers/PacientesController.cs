using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Route("api/pacientes")]
    public class PacientesController : ControllerBase
    {
        private readonly PacienteService _pacienteService;

        public PacientesController(PacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet]
        public ActionResult<List<Paciente>> ObtenerTodos()
        {
            var pacientes = _pacienteService.ObtenerTodos();
            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public ActionResult<Paciente> ObtenerPorId(string id)
        {
            var paciente = _pacienteService.ObtenerPorId(id);

            if (paciente == null)
            {
                return NotFound(new { mensaje = $"No se encontró el paciente con id {id}." });
            }

            return Ok(paciente);
        }

        [HttpPost]
        public ActionResult<Paciente> Crear(Paciente paciente)
        {
            _pacienteService.Crear(paciente);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = paciente.Id }, paciente);
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(string id, Paciente paciente)
        {
            paciente.Id = id;

            var actualizado = _pacienteService.Actualizar(paciente);

            if (!actualizado)
            {
                return NotFound(new { mensaje = $"No se encontró el paciente con id {id}." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(string id)
        {
            var eliminado = _pacienteService.Eliminar(id);

            if (!eliminado)
            {
                return NotFound(new { mensaje = $"No se encontró el paciente con id {id}." });
            }

            return NoContent();
        }
    }
}
