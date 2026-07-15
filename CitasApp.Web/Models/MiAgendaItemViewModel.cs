using CitasApp.Domain.Models;

namespace CitasApp.Web.Models
{
    public sealed record MiAgendaItemViewModel(Cita Cita, string PacienteNombre);
}
