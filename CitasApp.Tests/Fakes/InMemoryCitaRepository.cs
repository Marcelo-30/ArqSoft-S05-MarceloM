using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Tests.Fakes;

internal sealed class InMemoryCitaRepository : ICitaRepository
{
    private readonly List<Cita> _appointments;

    public InMemoryCitaRepository(IEnumerable<Cita>? appointments = null)
    {
        _appointments = appointments?.ToList() ?? [];
    }

    public IReadOnlyList<Cita> Appointments => _appointments;

    public int DeleteByPatientCallCount { get; private set; }

    public int DeleteByDoctorCallCount { get; private set; }

    public Task<IReadOnlyList<Cita>> ObtenerTodasAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Cita>>(_appointments.ToList());
    }

    public Task<Cita?> ObtenerPorIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_appointments.FirstOrDefault(appointment => appointment.Id == id));
    }

    public Task<IReadOnlyList<Cita>> ObtenerPorPacienteAsync(
        string pacienteId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Cita>>(
            _appointments.Where(appointment => appointment.PacienteId == pacienteId).ToList());
    }

    public Task<IReadOnlyList<Cita>> ObtenerPorMedicoAsync(
        string medicoId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Cita>>(
            _appointments.Where(appointment => appointment.MedicoId == medicoId).ToList());
    }

    public Task<bool> ExisteEnHorarioAsync(
        string medicoId,
        DateOnly fecha,
        TimeOnly hora,
        string? citaIdExcluida = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool exists = _appointments.Any(appointment =>
            appointment.Id != citaIdExcluida &&
            appointment.MedicoId == medicoId &&
            appointment.Fecha == fecha &&
            appointment.Hora == hora &&
            !appointment.Estado.Equals(EstadosCita.Cancelada, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task AgregarAsync(Cita cita, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _appointments.Add(cita);
        return Task.CompletedTask;
    }

    public Task<bool> ActualizarAsync(
        Cita cita,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        int index = _appointments.FindIndex(appointment => appointment.Id == cita.Id);
        if (index < 0)
        {
            return Task.FromResult(false);
        }

        _appointments[index] = cita;
        return Task.FromResult(true);
    }

    public Task<bool> EliminarAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_appointments.RemoveAll(appointment => appointment.Id == id) > 0);
    }

    public Task<int> EliminarPorPacienteAsync(
        string pacienteId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        DeleteByPatientCallCount++;
        return Task.FromResult(
            _appointments.RemoveAll(appointment => appointment.PacienteId == pacienteId));
    }

    public Task<int> EliminarPorMedicoAsync(
        string medicoId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        DeleteByDoctorCallCount++;
        return Task.FromResult(
            _appointments.RemoveAll(appointment => appointment.MedicoId == medicoId));
    }
}
