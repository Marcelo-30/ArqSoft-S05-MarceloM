using System.ComponentModel.DataAnnotations;

namespace CitasApp.Api.Dtos
{
    public sealed class CrearUsuarioRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = string.Empty;

        public string? MedicoId { get; set; }
    }
}
