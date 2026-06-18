using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Infrastructure.Repositories
{
    public class JsonCitaRepository : JsonRepository<Cita>, ICitaRepository
    {
        public JsonCitaRepository(string filePath) : base(filePath)
        {
        }
    }
}
