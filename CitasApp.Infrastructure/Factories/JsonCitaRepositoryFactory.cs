using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Repositories;

namespace CitasApp.Infrastructure.Factories
{
    public sealed class JsonCitaRepositoryFactory : JsonRepositoryFactory<ICitaRepository>
    {
        public JsonCitaRepositoryFactory(string contentRootPath) : base(contentRootPath)
        {
        }

        protected override string NombreArchivo => "citas.json";

        protected override ICitaRepository CrearRepositorio(string filePath)
        {
            return new JsonCitaRepository(filePath);
        }
    }
}
