using CitasApp.Domain.Models;

namespace CitasApp.Domain.Interfaces
{
    public interface IMedicoRepository
    {
        Task<IReadOnlyList<Medico>> ObtenerTodosAsync(CancellationToken cancellationToken = default);

        Task<Medico?> ObtenerPorIdAsync(string id, CancellationToken cancellationToken = default);

        Task AgregarAsync(Medico medico, CancellationToken cancellationToken = default);

        Task<bool> ActualizarAsync(Medico medico, CancellationToken cancellationToken = default);

        Task<bool> EliminarAsync(string id, CancellationToken cancellationToken = default);
    }
}
