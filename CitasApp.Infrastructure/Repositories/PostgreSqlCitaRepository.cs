using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Persistence;

namespace CitasApp.Infrastructure.Repositories
{
    public class PostgreSqlCitaRepository
        : PostgreSqlRepository<Cita>, ICitaRepository
    {
        public PostgreSqlCitaRepository(
            CitasAppDbContext context)
            : base(
                context,
                cita => cita.Id,
                (cita, id) => cita.Id = id)
        {
        }
    }
}