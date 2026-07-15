using CitasApp.Domain.Models;

namespace CitasApp.Domain.Interfaces
{
    public interface ICitaRepository
    {
        Task<IReadOnlyList<Cita>> ObtenerTodasAsync(CancellationToken cancellationToken = default);

        Task<Cita?> ObtenerPorIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Cita>> ObtenerPorPacienteAsync(
            string pacienteId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Cita>> ObtenerPorMedicoAsync(
            string medicoId,
            CancellationToken cancellationToken = default);

        Task<bool> ExisteEnHorarioAsync(
            string medicoId,
            DateOnly fecha,
            TimeOnly hora,
            string? citaIdExcluida = null,
            CancellationToken cancellationToken = default);

        Task AgregarAsync(Cita cita, CancellationToken cancellationToken = default);

        Task<bool> ActualizarAsync(Cita cita, CancellationToken cancellationToken = default);

        Task<bool> EliminarAsync(string id, CancellationToken cancellationToken = default);

        Task<int> EliminarPorPacienteAsync(
            string pacienteId,
            CancellationToken cancellationToken = default);

        Task<int> EliminarPorMedicoAsync(
            string medicoId,
            CancellationToken cancellationToken = default);
    }
}
