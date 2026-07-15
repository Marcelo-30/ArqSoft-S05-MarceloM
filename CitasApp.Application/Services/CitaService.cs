using CitasApp.Application.Exceptions;
using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Application.Services
{
    public sealed class CitaService
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IMedicoRepository _medicoRepository;

        public CitaService(
            ICitaRepository citaRepository,
            IPacienteRepository pacienteRepository,
            IMedicoRepository medicoRepository)
        {
            _citaRepository = citaRepository;
            _pacienteRepository = pacienteRepository;
            _medicoRepository = medicoRepository;
        }

        public Task<IReadOnlyList<Cita>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default)
        {
            return _citaRepository.ObtenerTodasAsync(cancellationToken);
        }

        public Task<Cita?> ObtenerPorIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            return _citaRepository.ObtenerPorIdAsync(id, cancellationToken);
        }

        public Task<IReadOnlyList<Cita>> ObtenerPorPacienteAsync(
            string pacienteId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(pacienteId);
            return _citaRepository.ObtenerPorPacienteAsync(pacienteId, cancellationToken);
        }

        public Task<IReadOnlyList<Cita>> ObtenerPorMedicoAsync(
            string medicoId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(medicoId);
            return _citaRepository.ObtenerPorMedicoAsync(medicoId, cancellationToken);
        }

        public async Task CrearAsync(Cita cita, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(cita);

            cita.Id = Guid.NewGuid().ToString();
            cita.Estado = NormalizarEstado(cita.Estado);

            await ValidarAsync(cita, null, cancellationToken);
            await _citaRepository.AgregarAsync(cita, cancellationToken);
        }

        public async Task<bool> ActualizarAsync(
            Cita cita,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(cita);
            ArgumentException.ThrowIfNullOrWhiteSpace(cita.Id);

            cita.Estado = NormalizarEstado(cita.Estado);
            await ValidarAsync(cita, cita.Id, cancellationToken);

            return await _citaRepository.ActualizarAsync(cita, cancellationToken);
        }

        public Task<bool> EliminarAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            return _citaRepository.EliminarAsync(id, cancellationToken);
        }

        private async Task ValidarAsync(
            Cita cita,
            string? citaIdExcluida,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cita.PacienteId) ||
                await _pacienteRepository.ObtenerPorIdAsync(cita.PacienteId, cancellationToken) is null)
            {
                throw new ValidacionCitaException("El paciente seleccionado no existe.");
            }

            if (string.IsNullOrWhiteSpace(cita.MedicoId) ||
                await _medicoRepository.ObtenerPorIdAsync(cita.MedicoId, cancellationToken) is null)
            {
                throw new ValidacionCitaException("El medico seleccionado no existe.");
            }

            if (cita.Fecha == default)
            {
                throw new ValidacionCitaException("La fecha de la cita es obligatoria.");
            }

            if (!EstadosCita.Todos.Contains(cita.Estado))
            {
                throw new ValidacionCitaException("El estado de la cita no es valido.");
            }

            if (await _citaRepository.ExisteEnHorarioAsync(
                cita.MedicoId,
                cita.Fecha,
                cita.Hora,
                citaIdExcluida,
                cancellationToken))
            {
                throw new ConflictoHorarioCitaException();
            }
        }

        private static string NormalizarEstado(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
            {
                return EstadosCita.Pendiente;
            }

            return EstadosCita.Todos.FirstOrDefault(
                estadoValido => estadoValido.Equals(estado.Trim(), StringComparison.OrdinalIgnoreCase))
                ?? estado.Trim();
        }
    }
}
