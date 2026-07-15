using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Infrastructure.Repositories
{
    public sealed class MemoriaPacienteRepository : IPacienteRepository
    {
        private readonly Lock _lock = new();
        private readonly List<Paciente> _pacientes =
        [
            new() { Id = "P1", Nombre = "Carlos", Apellido = "Ramirez", Email = "carlos.ramirez@gmail.com", Telefono = "9991234567" },
            new() { Id = "P2", Nombre = "Ana", Apellido = "Lopez", Email = "ana.lopez@gmail.com", Telefono = "9997654321" },
            new() { Id = "P3", Nombre = "Luis", Apellido = "Martinez", Email = "luis.martinez@gmail.com", Telefono = "9991112233" },
            new() { Id = "P4", Nombre = "Maria", Apellido = "Gomez", Email = "maria.gomez@gmail.com", Telefono = "9994445566" }
        ];

        public Task<IReadOnlyList<Paciente>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (_lock)
            {
                IReadOnlyList<Paciente> resultado = _pacientes.Select(Copiar).ToList();
                return Task.FromResult(resultado);
            }
        }

        public Task<Paciente?> ObtenerPorIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (_lock)
            {
                Paciente? paciente = _pacientes.FirstOrDefault(actual => actual.Id == id);
                return Task.FromResult(paciente is null ? null : Copiar(paciente));
            }
        }

        public Task AgregarAsync(Paciente paciente, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (_lock)
            {
                _pacientes.Add(Copiar(paciente));
            }

            return Task.CompletedTask;
        }

        public Task<bool> ActualizarAsync(
            Paciente paciente,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (_lock)
            {
                int indice = _pacientes.FindIndex(actual => actual.Id == paciente.Id);
                if (indice < 0)
                {
                    return Task.FromResult(false);
                }

                _pacientes[indice] = Copiar(paciente);
                return Task.FromResult(true);
            }
        }

        public Task<bool> EliminarAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (_lock)
            {
                return Task.FromResult(_pacientes.RemoveAll(paciente => paciente.Id == id) > 0);
            }
        }

        private static Paciente Copiar(Paciente paciente)
        {
            return new Paciente
            {
                Id = paciente.Id,
                Nombre = paciente.Nombre,
                Apellido = paciente.Apellido,
                Email = paciente.Email,
                Telefono = paciente.Telefono
            };
        }
    }
}
