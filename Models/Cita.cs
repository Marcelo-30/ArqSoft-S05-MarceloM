namespace CitasApp.Models
{
    public class Cita
    {
        public string Id { get; set; }

        public string PacienteId { get; set; }

        public string MedicoId { get; set; }

        public DateOnly Fecha { get; set; }

        public TimeOnly Hora { get; set; }

        public string Motivo { get; set; }

        public string Estado { get; set; }

    }
}
