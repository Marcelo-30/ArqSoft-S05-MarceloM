namespace CitasApp.Domain.Models
{
    public class Cita
    {
        public string Id { get; set; } = string.Empty;

        public string PacienteId { get; set; } = string.Empty;

        public string MedicoId { get; set; } = string.Empty;

        public DateOnly Fecha { get; set; }

        public TimeOnly Hora { get; set; }

        public string Motivo { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;
    }
}
