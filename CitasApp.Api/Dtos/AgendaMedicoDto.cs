namespace CitasApp.Api.Dtos
{
    public class AgendaMedicoDto
    {
        public string CitaId { get; set; } = string.Empty;

        public DateOnly Fecha { get; set; }

        public TimeOnly Hora { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string Motivo { get; set; } = string.Empty;

        public string MedicoId { get; set; } = string.Empty;

        public string MedicoNombre { get; set; } = string.Empty;

        public string PacienteId { get; set; } = string.Empty;

        public string PacienteNombre { get; set; } = string.Empty;

        public string PacienteTelefono { get; set; } = string.Empty;
    }
}
