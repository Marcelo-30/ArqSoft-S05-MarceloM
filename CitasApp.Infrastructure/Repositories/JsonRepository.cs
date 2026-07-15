using System.Collections.Concurrent;
using System.Text.Json;

namespace CitasApp.Infrastructure.Repositories
{
    public abstract class JsonRepository<T>
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> FileLocks = new();
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        private readonly string _filePath;
        private readonly SemaphoreSlim _fileLock;
        private readonly Func<T, string> _obtenerId;

        protected JsonRepository(string filePath, Func<T, string> obtenerId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
            ArgumentNullException.ThrowIfNull(obtenerId);

            _filePath = Path.GetFullPath(filePath);
            _obtenerId = obtenerId;
            _fileLock = FileLocks.GetOrAdd(_filePath, _ => new SemaphoreSlim(1, 1));

            string? directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        protected async Task<IReadOnlyList<T>> ListarAsync(
            CancellationToken cancellationToken = default)
        {
            await _fileLock.WaitAsync(cancellationToken);
            try
            {
                return await LeerSinBloqueoAsync(cancellationToken);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        protected async Task<T?> BuscarAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<T> datos = await ListarAsync(cancellationToken);
            return datos.FirstOrDefault(registro => _obtenerId(registro) == id);
        }

        protected Task AgregarRegistroAsync(T registro, CancellationToken cancellationToken = default)
        {
            return ModificarAsync(
                datos =>
                {
                    if (datos.Any(actual => _obtenerId(actual) == _obtenerId(registro)))
                    {
                        throw new InvalidOperationException("Ya existe un registro con el mismo identificador.");
                    }

                    datos.Add(registro);
                },
                cancellationToken);
        }

        protected async Task<bool> ActualizarRegistroAsync(
            T registro,
            CancellationToken cancellationToken = default)
        {
            bool actualizado = false;
            await ModificarAsync(
                datos =>
                {
                    int indice = datos.FindIndex(actual => _obtenerId(actual) == _obtenerId(registro));
                    if (indice >= 0)
                    {
                        datos[indice] = registro;
                        actualizado = true;
                    }
                },
                cancellationToken);

            return actualizado;
        }

        protected async Task<bool> EliminarRegistroAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            int eliminados = await EliminarRegistrosAsync(
                registro => _obtenerId(registro) == id,
                cancellationToken);

            return eliminados > 0;
        }

        protected async Task<int> EliminarRegistrosAsync(
            Predicate<T> predicate,
            CancellationToken cancellationToken = default)
        {
            int eliminados = 0;
            await ModificarAsync(
                datos => eliminados = datos.RemoveAll(predicate),
                cancellationToken);

            return eliminados;
        }

        private async Task ModificarAsync(
            Action<List<T>> modificar,
            CancellationToken cancellationToken)
        {
            await _fileLock.WaitAsync(cancellationToken);
            try
            {
                List<T> datos = await LeerSinBloqueoAsync(cancellationToken);
                modificar(datos);
                await GuardarSinBloqueoAsync(datos, cancellationToken);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private async Task<List<T>> LeerSinBloqueoAsync(CancellationToken cancellationToken)
        {
            string json = await File.ReadAllTextAsync(_filePath, cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return [];
            }

            return JsonSerializer.Deserialize<List<T>>(json) ?? [];
        }

        private async Task GuardarSinBloqueoAsync(
            IReadOnlyCollection<T> datos,
            CancellationToken cancellationToken)
        {
            string temporal = $"{_filePath}.{Guid.NewGuid():N}.tmp";
            try
            {
                string json = JsonSerializer.Serialize(datos, JsonOptions);
                await File.WriteAllTextAsync(temporal, json, cancellationToken);
                File.Move(temporal, _filePath, true);
            }
            finally
            {
                if (File.Exists(temporal))
                {
                    File.Delete(temporal);
                }
            }
        }
    }
}
