using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Infrastructure.Repositories
{
    public sealed class JsonMedicoRepository : JsonRepository<Medico>, IMedicoRepository
    {
        public JsonMedicoRepository(string filePath)
            : base(filePath, medico => medico.Id)
        {
        }

        public Task<IReadOnlyList<Medico>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default) => ListarAsync(cancellationToken);

        public Task<Medico?> ObtenerPorIdAsync(
            string id,
            CancellationToken cancellationToken = default) => BuscarAsync(id, cancellationToken);

        public Task AgregarAsync(
            Medico medico,
            CancellationToken cancellationToken = default) => AgregarRegistroAsync(medico, cancellationToken);

        public Task<bool> ActualizarAsync(
            Medico medico,
            CancellationToken cancellationToken = default) => ActualizarRegistroAsync(medico, cancellationToken);

        public Task<bool> EliminarAsync(
            string id,
            CancellationToken cancellationToken = default) => EliminarRegistroAsync(id, cancellationToken);
    }
}
