using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Infrastructure.Repositories
{
    public sealed class PostgreSqlCitaRepository : ICitaRepository
    {
        private readonly CitasAppDbContext _context;

        public PostgreSqlCitaRepository(CitasAppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Cita>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default)
        {
            return await ConsultaOrdenada().ToListAsync(cancellationToken);
        }

        public Task<Cita?> ObtenerPorIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return _context.Citas
                .AsNoTracking()
                .SingleOrDefaultAsync(cita => cita.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Cita>> ObtenerPorPacienteAsync(
            string pacienteId,
            CancellationToken cancellationToken = default)
        {
            return await ConsultaOrdenada()
                .Where(cita => cita.PacienteId == pacienteId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Cita>> ObtenerPorMedicoAsync(
            string medicoId,
            CancellationToken cancellationToken = default)
        {
            return await ConsultaOrdenada()
                .Where(cita => cita.MedicoId == medicoId)
                .ToListAsync(cancellationToken);
        }

        public Task<bool> ExisteEnHorarioAsync(
            string medicoId,
            DateOnly fecha,
            TimeOnly hora,
            string? citaIdExcluida = null,
            CancellationToken cancellationToken = default)
        {
            return _context.Citas.AnyAsync(
                cita => cita.MedicoId == medicoId &&
                    cita.Fecha == fecha &&
                    cita.Hora == hora &&
                    cita.Estado != EstadosCita.Cancelada &&
                    (citaIdExcluida == null || cita.Id != citaIdExcluida),
                cancellationToken);
        }

        public async Task AgregarAsync(Cita cita, CancellationToken cancellationToken = default)
        {
            await _context.Citas.AddAsync(cita, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ActualizarAsync(
            Cita cita,
            CancellationToken cancellationToken = default)
        {
            Cita? existente = await _context.Citas
                .SingleOrDefaultAsync(actual => actual.Id == cita.Id, cancellationToken);

            if (existente is null)
            {
                return false;
            }

            existente.PacienteId = cita.PacienteId;
            existente.MedicoId = cita.MedicoId;
            existente.Fecha = cita.Fecha;
            existente.Hora = cita.Hora;
            existente.Motivo = cita.Motivo;
            existente.Estado = cita.Estado;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EliminarAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            int eliminadas = await _context.Citas
                .Where(cita => cita.Id == id)
                .ExecuteDeleteAsync(cancellationToken);

            return eliminadas > 0;
        }

        public Task<int> EliminarPorPacienteAsync(
            string pacienteId,
            CancellationToken cancellationToken = default)
        {
            return _context.Citas
                .Where(cita => cita.PacienteId == pacienteId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public Task<int> EliminarPorMedicoAsync(
            string medicoId,
            CancellationToken cancellationToken = default)
        {
            return _context.Citas
                .Where(cita => cita.MedicoId == medicoId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        private IOrderedQueryable<Cita> ConsultaOrdenada()
        {
            return _context.Citas
                .AsNoTracking()
                .OrderBy(cita => cita.Fecha)
                .ThenBy(cita => cita.Hora);
        }
    }
}
