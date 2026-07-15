using System.ComponentModel.DataAnnotations;

namespace CitasApp.Domain.Models
{
    public class Medico
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        [StringLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El numero de licencia es obligatorio.")]
        [StringLength(50)]
        public string NumeroLicencia { get; set; } = string.Empty;
    }
}
