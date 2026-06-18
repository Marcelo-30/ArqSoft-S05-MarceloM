using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Infrastructure.Repositories
{
    public class JsonMedicoRepository : JsonRepository<Medico>, IMedicoRepository
    {
        public JsonMedicoRepository(string filePath) : base(filePath)
        {
        }
    }
}
