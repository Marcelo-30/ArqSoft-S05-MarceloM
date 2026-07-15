using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Infrastructure.Repositories
{
    public sealed class JsonCitaRepository : JsonRepository<Cita>, ICitaRepository
    {
        public JsonCitaRepository(string filePath)
            : base(filePath, cita => cita.Id)
        {
        }

        public Task<IReadOnlyList<Cita>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default) => ListarAsync(cancellationToken);

        public Task<Cita?> ObtenerPorIdAsync(
            string id,
            CancellationToken cancellationToken = default) => BuscarAsync(id, cancellationToken);

        public async Task<IReadOnlyList<Cita>> ObtenerPorPacienteAsync(
            string pacienteId,
            CancellationToken cancellationToken = default)
        {
            return (await ListarAsync(cancellationToken))
                .Where(cita => cita.PacienteId == pacienteId)
                .OrderBy(cita => cita.Fecha)
                .ThenBy(cita => cita.Hora)
                .ToList();
        }

        public async Task<IReadOnlyList<Cita>> ObtenerPorMedicoAsync(
            string medicoId,
            CancellationToken cancellationToken = default)
        {
            return (await ListarAsync(cancellationToken))
                .Where(cita => cita.MedicoId == medicoId)
                .OrderBy(cita => cita.Fecha)
                .ThenBy(cita => cita.Hora)
                .ToList();
        }

        public async Task<bool> ExisteEnHorarioAsync(
            string medicoId,
            DateOnly fecha,
            TimeOnly hora,
            string? citaIdExcluida = null,
            CancellationToken cancellationToken = default)
        {
            return (await ListarAsync(cancellationToken)).Any(
                cita => cita.MedicoId == medicoId &&
                    cita.Fecha == fecha &&
                    cita.Hora == hora &&
                    !cita.Estado.Equals(EstadosCita.Cancelada, StringComparison.OrdinalIgnoreCase) &&
                    (citaIdExcluida is null || cita.Id != citaIdExcluida));
        }

        public Task AgregarAsync(
            Cita cita,
            CancellationToken cancellationToken = default) => AgregarRegistroAsync(cita, cancellationToken);

        public Task<bool> ActualizarAsync(
            Cita cita,
            CancellationToken cancellationToken = default) => ActualizarRegistroAsync(cita, cancellationToken);

        public Task<bool> EliminarAsync(
            string id,
            CancellationToken cancellationToken = default) => EliminarRegistroAsync(id, cancellationToken);

        public Task<int> EliminarPorPacienteAsync(
            string pacienteId,
            CancellationToken cancellationToken = default) =>
            EliminarRegistrosAsync(cita => cita.PacienteId == pacienteId, cancellationToken);

        public Task<int> EliminarPorMedicoAsync(
            string medicoId,
            CancellationToken cancellationToken = default) =>
            EliminarRegistrosAsync(cita => cita.MedicoId == medicoId, cancellationToken);
    }
}
