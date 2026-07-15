using CitasApp.Application.Services;
using CitasApp.Application.Security;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/medicos")]
    public class MedicosController : ControllerBase
    {
        private readonly MedicoService _medicoService;

        public MedicosController(MedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        [HttpGet]
        [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
        public async Task<ActionResult<IReadOnlyList<Medico>>> ObtenerTodos(
            CancellationToken cancellationToken)
        {
            return Ok(await _medicoService.ObtenerTodosAsync(cancellationToken));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RolesAplicacion.Administrador + "," + RolesAplicacion.Recepcionista)]
        public async Task<ActionResult<Medico>> ObtenerPorId(
            string id,
            CancellationToken cancellationToken)
        {
            Medico? medico = await _medicoService.ObtenerPorIdAsync(id, cancellationToken);
            return medico is null
                ? NotFound(new { mensaje = $"No se encontro el medico con id {id}." })
                : Ok(medico);
        }

        [HttpPost]
        [Authorize(Roles = RolesAplicacion.Administrador)]
        public async Task<ActionResult<Medico>> Crear(
            Medico medico,
            CancellationToken cancellationToken)
        {
            await _medicoService.CrearAsync(medico, cancellationToken);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = medico.Id }, medico);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RolesAplicacion.Administrador)]
        public async Task<IActionResult> Actualizar(
            string id,
            Medico medico,
            CancellationToken cancellationToken)
        {
            medico.Id = id;
            bool actualizado = await _medicoService.ActualizarAsync(medico, cancellationToken);
            return actualizado
                ? NoContent()
                : NotFound(new { mensaje = $"No se encontro el medico con id {id}." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RolesAplicacion.Administrador)]
        public async Task<IActionResult> Eliminar(string id, CancellationToken cancellationToken)
        {
            bool eliminado = await _medicoService.EliminarAsync(id, cancellationToken);
            return eliminado
                ? NoContent()
                : NotFound(new { mensaje = $"No se encontro el medico con id {id}." });
        }
    }
}
