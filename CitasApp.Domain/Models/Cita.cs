using System.ComponentModel.DataAnnotations;

namespace CitasApp.Domain.Models
{
    public class Cita
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "El paciente es obligatorio.")]
        public string PacienteId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El medico es obligatorio.")]
        public string MedicoId { get; set; } = string.Empty;

        public DateOnly Fecha { get; set; }

        public TimeOnly Hora { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [RegularExpression("^(Pendiente|Confirmada|Cancelada)$", ErrorMessage = "El estado de la cita no es valido.")]
        [StringLength(30)]
        public string Estado { get; set; } = string.Empty;
    }
}
