using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Infrastructure.Repositories
{
    public sealed class JsonPacienteRepository : JsonRepository<Paciente>, IPacienteRepository
    {
        public JsonPacienteRepository(string filePath)
            : base(filePath, paciente => paciente.Id)
        {
        }

        public Task<IReadOnlyList<Paciente>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default) => ListarAsync(cancellationToken);

        public Task<Paciente?> ObtenerPorIdAsync(
            string id,
            CancellationToken cancellationToken = default) => BuscarAsync(id, cancellationToken);

        public Task AgregarAsync(
            Paciente paciente,
            CancellationToken cancellationToken = default) => AgregarRegistroAsync(paciente, cancellationToken);

        public Task<bool> ActualizarAsync(
            Paciente paciente,
            CancellationToken cancellationToken = default) => ActualizarRegistroAsync(paciente, cancellationToken);

        public Task<bool> EliminarAsync(
            string id,
            CancellationToken cancellationToken = default) => EliminarRegistroAsync(id, cancellationToken);
    }
}
