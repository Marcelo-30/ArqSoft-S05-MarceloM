using CitasApp.Application.Strategies.Calculadora;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Route("api/calculadora")]
    public class CalculadoraController : ControllerBase
    {
        private readonly IReadOnlyDictionary<string, IOperacionCalculadora> _operaciones;

        public CalculadoraController(IEnumerable<IOperacionCalculadora> operaciones)
        {
            _operaciones = operaciones.ToDictionary(
                operacion => operacion.Nombre,
                StringComparer.OrdinalIgnoreCase);
        }

        [HttpGet]
        public IActionResult Calcular(
            [FromQuery] double? numero1,
            [FromQuery] double? numero2,
            [FromQuery] string? operacion)
        {
            if (numero1 == null || numero2 == null)
            {
                return BadRequest(new { mensaje = "Debes enviar numero1 y numero2." });
            }

            if (string.IsNullOrWhiteSpace(operacion))
            {
                return BadRequest(new { mensaje = "Debes enviar la operacion: suma, resta, multiplicacion o division." });
            }

            var operacionNormalizada = operacion.Trim().ToLowerInvariant();

            if (!_operaciones.TryGetValue(operacionNormalizada, out var operacionCalculadora))
            {
                return BadRequest(new { mensaje = "Operacion no valida. Usa suma, resta, multiplicacion o division." });
            }

            var resultadoOperacion = operacionCalculadora.Ejecutar(numero1.Value, numero2.Value);

            if (!resultadoOperacion.Exitoso)
            {
                return BadRequest(new { mensaje = resultadoOperacion.MensajeError });
            }

            return Ok(new
            {
                numero1 = numero1.Value,
                numero2 = numero2.Value,
                operacion = operacionCalculadora.Nombre,
                simbolo = operacionCalculadora.Simbolo,
                resultado = resultadoOperacion.Resultado
            });
        }
    }
}
