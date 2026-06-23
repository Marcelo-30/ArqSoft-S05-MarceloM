using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Route("api/calculadora")]
    public class CalculadoraController : ControllerBase
    {
        [HttpGet]
        public IActionResult Calcular(
            [FromQuery] double? numero1,
            [FromQuery] double? numero2,
            [FromQuery] string operacion)
        {
            if (numero1 == null || numero2 == null)
            {
                return BadRequest(new { mensaje = "Debes enviar numero1 y numero2." });
            }

            if (string.IsNullOrWhiteSpace(operacion))
            {
                return BadRequest(new { mensaje = "Debes enviar la operación: suma, resta, multiplicacion o division." });
            }

            var operacionNormalizada = operacion.Trim().ToLowerInvariant();
            double resultado;
            string simbolo;

            switch (operacionNormalizada)
            {
                case "suma":
                    resultado = numero1.Value + numero2.Value;
                    simbolo = "+";
                    break;
                case "resta":
                    resultado = numero1.Value - numero2.Value;
                    simbolo = "-";
                    break;
                case "multiplicacion":
                    resultado = numero1.Value * numero2.Value;
                    simbolo = "×";
                    break;
                case "division":
                    if (numero2.Value == 0)
                    {
                        return BadRequest(new { mensaje = "No se puede dividir entre cero." });
                    }

                    resultado = numero1.Value / numero2.Value;
                    simbolo = "÷";
                    break;
                default:
                    return BadRequest(new { mensaje = "Operación no válida. Usa suma, resta, multiplicacion o division." });
            }

            return Ok(new
            {
                numero1 = numero1.Value,
                numero2 = numero2.Value,
                operacion = operacionNormalizada,
                simbolo,
                resultado
            });
        }
    }
}
