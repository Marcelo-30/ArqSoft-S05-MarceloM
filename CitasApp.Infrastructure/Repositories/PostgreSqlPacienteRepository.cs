using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Infrastructure.Repositories
{
    public sealed class PostgreSqlPacienteRepository : IPacienteRepository
    {
        private readonly CitasAppDbContext _context;

        public PostgreSqlPacienteRepository(CitasAppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Paciente>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Pacientes
                .AsNoTracking()
                .OrderBy(paciente => paciente.Apellido)
                .ThenBy(paciente => paciente.Nombre)
                .ToListAsync(cancellationToken);
        }

        public Task<Paciente?> ObtenerPorIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return _context.Pacientes
                .AsNoTracking()
                .SingleOrDefaultAsync(paciente => paciente.Id == id, cancellationToken);
        }

        public async Task AgregarAsync(
            Paciente paciente,
            CancellationToken cancellationToken = default)
        {
            await _context.Pacientes.AddAsync(paciente, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ActualizarAsync(
            Paciente paciente,
            CancellationToken cancellationToken = default)
        {
            Paciente? existente = await _context.Pacientes
                .SingleOrDefaultAsync(actual => actual.Id == paciente.Id, cancellationToken);

            if (existente is null)
            {
                return false;
            }

            existente.Nombre = paciente.Nombre;
            existente.Apellido = paciente.Apellido;
            existente.Email = paciente.Email;
            existente.Telefono = paciente.Telefono;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EliminarAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            int eliminados = await _context.Pacientes
                .Where(paciente => paciente.Id == id)
                .ExecuteDeleteAsync(cancellationToken);

            return eliminados > 0;
        }
    }
}
