using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Repositories;

namespace CitasApp.Infrastructure.Factories
{
    public sealed class JsonPacienteRepositoryFactory : JsonRepositoryFactory<IPacienteRepository>
    {
        public JsonPacienteRepositoryFactory(string contentRootPath) : base(contentRootPath)
        {
        }

        protected override string NombreArchivo => "pacientes.json";

        protected override IPacienteRepository CrearRepositorio(string filePath)
        {
            return new JsonPacienteRepository(filePath);
        }
    }
}
