using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Application.Services
{
    public class PacienteService
    {
        private readonly IPacienteRepository _pacienteRepository;
        private readonly ICitaRepository _citaRepository;

        public PacienteService(
            IPacienteRepository pacienteRepository,
            ICitaRepository citaRepository)
        {
            _pacienteRepository = pacienteRepository;
            _citaRepository = citaRepository;
        }

        public List<Paciente> ObtenerTodos()
        {
            return _pacienteRepository.Leer();
        }

        public Paciente? ObtenerPorId(string id)
        {
            return _pacienteRepository.Leer()
                .FirstOrDefault(p => p.Id == id);
        }

        public void Crear(Paciente paciente)
        {
            var pacientes = _pacienteRepository.Leer();

            paciente.Id = GenerarSiguienteId(pacientes);
            pacientes.Add(paciente);

            _pacienteRepository.Guardar(pacientes);
        }

        public bool Actualizar(Paciente paciente)
        {
            var pacientes = _pacienteRepository.Leer();
            var pacienteExistente = pacientes.FirstOrDefault(p => p.Id == paciente.Id);

            if (pacienteExistente == null)
            {
                return false;
            }

            pacienteExistente.Nombre = paciente.Nombre;
            pacienteExistente.Apellido = paciente.Apellido;
            pacienteExistente.Email = paciente.Email;
            pacienteExistente.Telefono = paciente.Telefono;

            _pacienteRepository.Guardar(pacientes);

            return true;
        }

        public bool Eliminar(string id)
        {
            var pacientes = _pacienteRepository.Leer();
            var paciente = pacientes.FirstOrDefault(p => p.Id == id);

            if (paciente == null)
            {
                return false;
            }

            pacientes.Remove(paciente);
            _pacienteRepository.Guardar(pacientes);

            var citas = _citaRepository.Leer();
            var citasActualizadas = citas
                .Where(c => c.PacienteId != id)
                .ToList();

            _citaRepository.Guardar(citasActualizadas);

            return true;
        }

        private static string GenerarSiguienteId(List<Paciente> pacientes)
        {
            var ultimoNumero = pacientes
                .Select(p => p.Id)
                .Where(id => !string.IsNullOrWhiteSpace(id) && id.StartsWith("P"))
                .Select(id => int.TryParse(id[1..], out var numero) ? numero : 0)
                .DefaultIfEmpty(0)
                .Max();

            return $"P{ultimoNumero + 1}";
        }
    }
}
