using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Persistence;

namespace CitasApp.Infrastructure.Repositories
{
    public class PostgreSqlMedicoRepository
        : PostgreSqlRepository<Medico>, IMedicoRepository
    {
        public PostgreSqlMedicoRepository(
            CitasAppDbContext context)
            : base(
                context,
                medico => medico.Id,
                (medico, id) => medico.Id = id)
        {
        }
    }
}
