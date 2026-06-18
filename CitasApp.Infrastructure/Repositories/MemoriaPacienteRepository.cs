using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Infrastructure.Repositories
{
    public class MemoriaPacienteRepository : IPacienteRepository
    {
        private readonly List<Paciente> _pacientes = new()
        {
            new Paciente
            {
                Id = "P1",
                Nombre = "Carlos",
                Apellido = "Ramírez",
                Email = "carlos.ramirez@gmail.com",
                Telefono = "9991234567"
            },
            new Paciente
            {
                Id = "P2",
                Nombre = "Ana",
                Apellido = "López",
                Email = "ana.lopez@gmail.com",
                Telefono = "9997654321"
            },
            new Paciente
            {
                Id = "P3",
                Nombre = "Luis",
                Apellido = "Martínez",
                Email = "luis.martinez@gmail.com",
                Telefono = "9991112233"
            },
            new Paciente
            {
                Id = "P4",
                Nombre = "María",
                Apellido = "Gómez",
                Email = "maria.gomez@gmail.com",
                Telefono = "9994445566"
            }
        };

        public List<Paciente> Leer()
        {
            return _pacientes
                .Select(Copiar)
                .ToList();
        }

        public void Guardar(List<Paciente> datos)
        {
            _pacientes.Clear();
            _pacientes.AddRange(datos.Select(Copiar));
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
