using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Tests.Fakes;

internal sealed class InMemoryPacienteRepository : IPacienteRepository
{
    private readonly List<Paciente> _patients;

    public InMemoryPacienteRepository(IEnumerable<Paciente>? patients = null)
    {
        _patients = patients?.ToList() ?? [];
    }

    public IReadOnlyList<Paciente> Patients => _patients;

    public Task<IReadOnlyList<Paciente>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Paciente>>(_patients.ToList());
    }

    public Task<Paciente?> ObtenerPorIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_patients.FirstOrDefault(patient => patient.Id == id));
    }

    public Task AgregarAsync(
        Paciente paciente,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _patients.Add(paciente);
        return Task.CompletedTask;
    }

    public Task<bool> ActualizarAsync(
        Paciente paciente,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        int index = _patients.FindIndex(patient => patient.Id == paciente.Id);
        if (index < 0)
        {
            return Task.FromResult(false);
        }

        _patients[index] = paciente;
        return Task.FromResult(true);
    }

    public Task<bool> EliminarAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_patients.RemoveAll(patient => patient.Id == id) > 0);
    }
}
