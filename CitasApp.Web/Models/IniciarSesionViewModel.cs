using System.ComponentModel.DataAnnotations;

namespace CitasApp.Web.Models
{
    public sealed class IniciarSesionViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato valido.")]
        [Display(Name = "Correo")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrasena es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contrasena")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Mantener la sesion iniciada")]
        public bool Recordarme { get; set; }
    }
}
