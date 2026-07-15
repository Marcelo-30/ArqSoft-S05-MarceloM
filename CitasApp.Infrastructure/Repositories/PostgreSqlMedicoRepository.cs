using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Infrastructure.Repositories
{
    public sealed class PostgreSqlMedicoRepository : IMedicoRepository
    {
        private readonly CitasAppDbContext _context;

        public PostgreSqlMedicoRepository(CitasAppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Medico>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Medicos
                .AsNoTracking()
                .OrderBy(medico => medico.Apellido)
                .ThenBy(medico => medico.Nombre)
                .ToListAsync(cancellationToken);
        }

        public Task<Medico?> ObtenerPorIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return _context.Medicos
                .AsNoTracking()
                .SingleOrDefaultAsync(medico => medico.Id == id, cancellationToken);
        }

        public async Task AgregarAsync(
            Medico medico,
            CancellationToken cancellationToken = default)
        {
            await _context.Medicos.AddAsync(medico, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ActualizarAsync(
            Medico medico,
            CancellationToken cancellationToken = default)
        {
            Medico? existente = await _context.Medicos
                .SingleOrDefaultAsync(actual => actual.Id == medico.Id, cancellationToken);

            if (existente is null)
            {
                return false;
            }

            existente.Nombre = medico.Nombre;
            existente.Apellido = medico.Apellido;
            existente.Especialidad = medico.Especialidad;
            existente.NumeroLicencia = medico.NumeroLicencia;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EliminarAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            int eliminados = await _context.Medicos
                .Where(medico => medico.Id == id)
                .ExecuteDeleteAsync(cancellationToken);

            return eliminados > 0;
        }
    }
}
