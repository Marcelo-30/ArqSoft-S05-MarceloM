using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Application.Services
{
    public sealed class PacienteService
    {
        private readonly IPacienteRepository _pacienteRepository;
        private readonly ICitaRepository _citaRepository;

        public PacienteService(
            IPacienteRepository pacienteRepository,
            ICitaRepository citaRepository)
        {
            _pacienteRepository = pacienteRepository;
            _citaRepository = citaRepository;
        }

        public Task<IReadOnlyList<Paciente>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return _pacienteRepository.ObtenerTodosAsync(cancellationToken);
        }

        public Task<Paciente?> ObtenerPorIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            return _pacienteRepository.ObtenerPorIdAsync(id, cancellationToken);
        }

        public Task CrearAsync(Paciente paciente, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(paciente);
            paciente.Id = Guid.NewGuid().ToString();
            return _pacienteRepository.AgregarAsync(paciente, cancellationToken);
        }

        public Task<bool> ActualizarAsync(
            Paciente paciente,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(paciente);
            ArgumentException.ThrowIfNullOrWhiteSpace(paciente.Id);
            return _pacienteRepository.ActualizarAsync(paciente, cancellationToken);
        }

        public async Task<bool> EliminarAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            if (await _pacienteRepository.ObtenerPorIdAsync(id, cancellationToken) is null)
            {
                return false;
            }

            await _citaRepository.EliminarPorPacienteAsync(id, cancellationToken);
            return await _pacienteRepository.EliminarAsync(id, cancellationToken);
        }
    }
}
