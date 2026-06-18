using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Application.Services
{
    public class CitaService
    {
        private readonly ICitaRepository _citaRepository;

        public CitaService(ICitaRepository citaRepository)
        {
            _citaRepository = citaRepository;
        }

        public List<Cita> ObtenerTodas()
        {
            return _citaRepository.Leer();
        }

        public Cita? ObtenerPorId(string id)
        {
            return _citaRepository.Leer()
                .FirstOrDefault(c => c.Id == id);
        }

        public List<Cita> ObtenerPorPaciente(string pacienteId)
        {
            return _citaRepository.Leer()
                .Where(c => c.PacienteId == pacienteId)
                .ToList();
        }

        public void Crear(Cita cita)
        {
            var citas = _citaRepository.Leer();

            cita.Id = GenerarSiguienteId(citas);

            if (string.IsNullOrWhiteSpace(cita.Estado))
            {
                cita.Estado = "Pendiente";
            }

            citas.Add(cita);
            _citaRepository.Guardar(citas);
        }

        public bool Actualizar(Cita cita)
        {
            var citas = _citaRepository.Leer();
            var citaExistente = citas.FirstOrDefault(c => c.Id == cita.Id);

            if (citaExistente == null)
            {
                return false;
            }

            citaExistente.PacienteId = cita.PacienteId;
            citaExistente.MedicoId = cita.MedicoId;
            citaExistente.Fecha = cita.Fecha;
            citaExistente.Hora = cita.Hora;
            citaExistente.Motivo = cita.Motivo;
            citaExistente.Estado = string.IsNullOrWhiteSpace(cita.Estado)
                ? "Pendiente"
                : cita.Estado;

            _citaRepository.Guardar(citas);

            return true;
        }

        public bool Eliminar(string id)
        {
            var citas = _citaRepository.Leer();
            var cita = citas.FirstOrDefault(c => c.Id == id);

            if (cita == null)
            {
                return false;
            }

            citas.Remove(cita);
            _citaRepository.Guardar(citas);

            return true;
        }

        private static string GenerarSiguienteId(List<Cita> citas)
        {
            var ultimoNumero = citas
                .Select(c => c.Id)
                .Where(id => !string.IsNullOrWhiteSpace(id) && id.StartsWith("C"))
                .Select(id => int.TryParse(id[1..], out var numero) ? numero : 0)
                .DefaultIfEmpty(0)
                .Max();

            return $"C{ultimoNumero + 1}";
        }
    }
}
