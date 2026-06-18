using System.Text.Json;

namespace CitasApp.Services
{
    public class JsonFileService<T> : IJsonFileService<T>
    {
        private readonly string _filePath;

        public JsonFileService(string filePath)
        {
            _filePath = filePath;

            var directory = Path.GetDirectoryName(_filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        public List<T> Leer()
        {
            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }

            var datos = JsonSerializer.Deserialize<List<T>>(json);

            return datos ?? new List<T>();
        }

        public void Guardar(List<T> datos)
        {
            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(datos, opciones);

            File.WriteAllText(_filePath, json);
        }
    }
}
