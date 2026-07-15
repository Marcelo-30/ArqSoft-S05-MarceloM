using System.ComponentModel.DataAnnotations;

namespace CitasApp.Domain.Models
{
    public class Paciente
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato valido.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El telefono es obligatorio.")]
        [Phone(ErrorMessage = "El telefono no tiene un formato valido.")]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;
    }
}
