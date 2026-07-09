using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Repositories;

namespace CitasApp.Infrastructure.Factories
{
    public sealed class JsonMedicoRepositoryFactory : JsonRepositoryFactory<IMedicoRepository>
    {
        public JsonMedicoRepositoryFactory(string contentRootPath) : base(contentRootPath)
        {
        }

        protected override string NombreArchivo => "medicos.json";

        protected override IMedicoRepository CrearRepositorio(string filePath)
        {
            return new JsonMedicoRepository(filePath);
        }
    }
}
