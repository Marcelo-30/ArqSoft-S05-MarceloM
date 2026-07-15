using System.ComponentModel.DataAnnotations;

namespace CitasApp.Api.Dtos
{
    public sealed class ActualizarEstadoCitaDto
    {
        [Required]
        [RegularExpression("^(Pendiente|Confirmada|Cancelada)$")]
        public string Estado { get; set; } = string.Empty;
    }
}
