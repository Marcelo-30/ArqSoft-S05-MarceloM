using CitasApp.Domain.Models;

namespace CitasApp.Domain.Interfaces
{
    public interface IPacienteRepository
    {
        Task<IReadOnlyList<Paciente>> ObtenerTodosAsync(CancellationToken cancellationToken = default);

        Task<Paciente?> ObtenerPorIdAsync(string id, CancellationToken cancellationToken = default);

        Task AgregarAsync(Paciente paciente, CancellationToken cancellationToken = default);

        Task<bool> ActualizarAsync(Paciente paciente, CancellationToken cancellationToken = default);

        Task<bool> EliminarAsync(string id, CancellationToken cancellationToken = default);
    }
}
