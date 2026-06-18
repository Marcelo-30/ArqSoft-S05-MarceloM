using CitasApp.Application.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Infrastructure.Repositories
{
    public class JsonPacienteRepository : JsonRepository<Paciente>, IPacienteRepository
    {
        public JsonPacienteRepository(string filePath) : base(filePath)
        {
        }
    }
}
