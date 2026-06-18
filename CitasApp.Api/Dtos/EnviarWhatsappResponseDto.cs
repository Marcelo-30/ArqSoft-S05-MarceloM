namespace CitasApp.Api.Dtos
{
    public class EnviarWhatsappResponseDto
    {
        public string CitaId { get; set; } = string.Empty;

        public string Paciente { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string Mensaje { get; set; } = string.Empty;

        public string WhatsappUrl { get; set; } = string.Empty;

        public bool Enviado { get; set; }

        public string Nota { get; set; } = string.Empty;
    }
}
