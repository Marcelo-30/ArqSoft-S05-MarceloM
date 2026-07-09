namespace CitasApp.Infrastructure.Factories
{
    public abstract class JsonRepositoryFactory<TRepository>
    {
        private readonly string _contentRootPath;

        protected JsonRepositoryFactory(string contentRootPath)
        {
            _contentRootPath = contentRootPath;
        }

        protected abstract string NombreArchivo { get; }

        public TRepository Crear()
        {
            var ruta = ObtenerRutaArchivoData(NombreArchivo);
            return CrearRepositorio(ruta);
        }

        protected abstract TRepository CrearRepositorio(string filePath);

        private string ObtenerRutaArchivoData(string nombreArchivo)
        {
            var rutaCompartidaConWeb = Path.GetFullPath(
                Path.Combine(_contentRootPath, "..", "CitasApp.Web", "Data", nombreArchivo));

            if (File.Exists(rutaCompartidaConWeb))
            {
                return rutaCompartidaConWeb;
            }

            var rutaLocal = Path.Combine(_contentRootPath, "Data", nombreArchivo);
            Directory.CreateDirectory(Path.GetDirectoryName(rutaLocal)!);

            if (!File.Exists(rutaLocal))
            {
                File.WriteAllText(rutaLocal, "[]");
            }

            return rutaLocal;
        }
    }
}
