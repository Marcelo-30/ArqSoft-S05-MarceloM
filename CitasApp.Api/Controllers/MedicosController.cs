using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Route("api/medicos")]
    public class MedicosController : ControllerBase
    {
        private readonly MedicoService _medicoService;

        public MedicosController(MedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        [HttpGet]
        public ActionResult<List<Medico>> ObtenerTodos()
        {
            var medicos = _medicoService.ObtenerTodos();
            return Ok(medicos);
        }

        [HttpGet("{id}")]
        public ActionResult<Medico> ObtenerPorId(string id)
        {
            var medico = _medicoService.ObtenerPorId(id);

            if (medico == null)
            {
                return NotFound(new { mensaje = $"No se encontró el médico con id {id}." });
            }

            return Ok(medico);
        }

        [HttpPost]
        public ActionResult<Medico> Crear(Medico medico)
        {
            _medicoService.Crear(medico);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = medico.Id }, medico);
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(string id, Medico medico)
        {
            medico.Id = id;
            var actualizado = _medicoService.Actualizar(medico);

            if (!actualizado)
            {
                return NotFound(new { mensaje = $"No se encontró el médico con id {id}." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(string id)
        {
            var eliminado = _medicoService.Eliminar(id);

            if (!eliminado)
            {
                return NotFound(new { mensaje = $"No se encontró el médico con id {id}." });
            }

            return NoContent();
        }
    }
}
