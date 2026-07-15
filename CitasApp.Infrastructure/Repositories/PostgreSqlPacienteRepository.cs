using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Persistence;

namespace CitasApp.Infrastructure.Repositories
{
    public class PostgreSqlPacienteRepository
        : PostgreSqlRepository<Paciente>, IPacienteRepository
    {
        public PostgreSqlPacienteRepository(
            CitasAppDbContext context)
            : base(
                context,
                paciente => paciente.Id,
                (paciente, id) => paciente.Id = id)
        {
        }
    }
}
