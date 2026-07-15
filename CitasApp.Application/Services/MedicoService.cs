using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Application.Services
{
    public sealed class MedicoService
    {
        private readonly IMedicoRepository _medicoRepository;
        private readonly ICitaRepository _citaRepository;

        public MedicoService(
            IMedicoRepository medicoRepository,
            ICitaRepository citaRepository)
        {
            _medicoRepository = medicoRepository;
            _citaRepository = citaRepository;
        }

        public Task<IReadOnlyList<Medico>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return _medicoRepository.ObtenerTodosAsync(cancellationToken);
        }

        public Task<Medico?> ObtenerPorIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            return _medicoRepository.ObtenerPorIdAsync(id, cancellationToken);
        }

        public Task CrearAsync(Medico medico, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(medico);
            medico.Id = Guid.NewGuid().ToString();
            return _medicoRepository.AgregarAsync(medico, cancellationToken);
        }

        public Task<bool> ActualizarAsync(
            Medico medico,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(medico);
            ArgumentException.ThrowIfNullOrWhiteSpace(medico.Id);
            return _medicoRepository.ActualizarAsync(medico, cancellationToken);
        }

        public async Task<bool> EliminarAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            if (await _medicoRepository.ObtenerPorIdAsync(id, cancellationToken) is null)
            {
                return false;
            }

            await _citaRepository.EliminarPorMedicoAsync(id, cancellationToken);
            return await _medicoRepository.EliminarAsync(id, cancellationToken);
        }
    }
}
