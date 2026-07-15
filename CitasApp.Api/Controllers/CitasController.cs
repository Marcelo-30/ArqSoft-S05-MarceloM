using System.Security.Claims;
using CitasApp.Api.Dtos;
using CitasApp.Application.Exceptions;
using CitasApp.Application.Security;
using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/citas")]
    public class CitasController : ControllerBase
    {
        private readonly CitaService _citaService;
        private readonly PacienteService _pacienteService;

        public CitasController(CitaService citaService, PacienteService pacienteService)
        {
            _citaService = citaService;
            _pacienteService = pacienteService;
        }

        [HttpGet]
        [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
        public async Task<ActionResult<IReadOnlyList<Cita>>> ObtenerTodas(
            CancellationToken cancellationToken)
        {
            return Ok(await _citaService.ObtenerTodasAsync(cancellationToken));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
        public async Task<ActionResult<Cita>> ObtenerPorId(
            string id,
            CancellationToken cancellationToken)
        {
            Cita? cita = await _citaService.ObtenerPorIdAsync(id, cancellationToken);
            return cita is null
                ? NotFound(new { mensaje = $"No se encontro la cita con id {id}." })
                : Ok(cita);
        }

        [HttpGet("paciente/{pacienteId}")]
        [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
        public async Task<ActionResult<IReadOnlyList<Cita>>> ObtenerPorPaciente(
            string pacienteId,
            CancellationToken cancellationToken)
        {
            if (await _pacienteService.ObtenerPorIdAsync(pacienteId, cancellationToken) is null)
            {
                return NotFound(new { mensaje = $"No se encontro el paciente con id {pacienteId}." });
            }

            return Ok(await _citaService.ObtenerPorPacienteAsync(pacienteId, cancellationToken));
        }

        [HttpPost]
        [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
        public async Task<ActionResult<Cita>> Crear(Cita cita, CancellationToken cancellationToken)
        {
            try
            {
                await _citaService.CrearAsync(cita, cancellationToken);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = cita.Id }, cita);
            }
            catch (ValidacionCitaException exception)
            {
                return BadRequest(new { mensaje = exception.Message });
            }
            catch (ConflictoHorarioCitaException exception)
            {
                return Conflict(new { mensaje = exception.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
        public async Task<IActionResult> Actualizar(
            string id,
            Cita cita,
            CancellationToken cancellationToken)
        {
            cita.Id = id;
            try
            {
                bool actualizado = await _citaService.ActualizarAsync(cita, cancellationToken);
                return actualizado
                    ? NoContent()
                    : NotFound(new { mensaje = $"No se encontro la cita con id {id}." });
            }
            catch (ValidacionCitaException exception)
            {
                return BadRequest(new { mensaje = exception.Message });
            }
            catch (ConflictoHorarioCitaException exception)
            {
                return Conflict(new { mensaje = exception.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
        public async Task<IActionResult> Eliminar(string id, CancellationToken cancellationToken)
        {
            bool eliminado = await _citaService.EliminarAsync(id, cancellationToken);
            return eliminado
                ? NoContent()
                : NotFound(new { mensaje = $"No se encontro la cita con id {id}." });
        }

        [HttpPatch("{id}/estado")]
        [Authorize(Roles = RolesAplicacion.Medico)]
        public async Task<IActionResult> ActualizarEstadoPropio(
            string id,
            ActualizarEstadoCitaDto request,
            CancellationToken cancellationToken)
        {
            string? medicoId = User.FindFirstValue("medico_id");
            if (string.IsNullOrWhiteSpace(medicoId))
            {
                return Forbid();
            }

            Cita? cita = await _citaService.ObtenerPorIdAsync(id, cancellationToken);
            if (cita is null || cita.MedicoId != medicoId)
            {
                return NotFound();
            }

            cita.Estado = request.Estado;
            try
            {
                await _citaService.ActualizarAsync(cita, cancellationToken);
                return NoContent();
            }
            catch (ConflictoHorarioCitaException exception)
            {
                return Conflict(new { mensaje = exception.Message });
            }
        }
    }
}
