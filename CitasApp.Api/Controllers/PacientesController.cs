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
        public async Task<ActionResult<IReadOnlyList<Paciente>>> ObtenerTodos(
            CancellationToken cancellationToken)
        {
            return Ok(await _pacienteService.ObtenerTodosAsync(cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Paciente>> ObtenerPorId(
            string id,
            CancellationToken cancellationToken)
        {
            Paciente? paciente = await _pacienteService.ObtenerPorIdAsync(id, cancellationToken);
            return paciente is null
                ? NotFound(new { mensaje = $"No se encontro el paciente con id {id}." })
                : Ok(paciente);
        }

        [HttpPost]
        public async Task<ActionResult<Paciente>> Crear(
            Paciente paciente,
            CancellationToken cancellationToken)
        {
            await _pacienteService.CrearAsync(paciente, cancellationToken);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = paciente.Id }, paciente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(
            string id,
            Paciente paciente,
            CancellationToken cancellationToken)
        {
            paciente.Id = id;
            bool actualizado = await _pacienteService.ActualizarAsync(paciente, cancellationToken);
            return actualizado
                ? NoContent()
                : NotFound(new { mensaje = $"No se encontro el paciente con id {id}." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(string id, CancellationToken cancellationToken)
        {
            bool eliminado = await _pacienteService.EliminarAsync(id, cancellationToken);
            return eliminado
                ? NoContent()
                : NotFound(new { mensaje = $"No se encontro el paciente con id {id}." });
        }
    }
}
