using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Tests.Fakes;

internal sealed class InMemoryMedicoRepository : IMedicoRepository
{
    private readonly List<Medico> _doctors;

    public InMemoryMedicoRepository(IEnumerable<Medico>? doctors = null)
    {
        _doctors = doctors?.ToList() ?? [];
    }

    public IReadOnlyList<Medico> Doctors => _doctors;

    public Task<IReadOnlyList<Medico>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Medico>>(_doctors.ToList());
    }

    public Task<Medico?> ObtenerPorIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_doctors.FirstOrDefault(doctor => doctor.Id == id));
    }

    public Task AgregarAsync(
        Medico medico,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _doctors.Add(medico);
        return Task.CompletedTask;
    }

    public Task<bool> ActualizarAsync(
        Medico medico,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        int index = _doctors.FindIndex(doctor => doctor.Id == medico.Id);
        if (index < 0)
        {
            return Task.FromResult(false);
        }

        _doctors[index] = medico;
        return Task.FromResult(true);
    }

    public Task<bool> EliminarAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_doctors.RemoveAll(doctor => doctor.Id == id) > 0);
    }
}
