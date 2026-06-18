using CitasApp.Domain.Interfaces;
using CitasApp.Domain.Models;

namespace CitasApp.Application.Services
{
    public class MedicoService
    {
        private readonly IMedicoRepository _medicoRepository;
        private readonly ICitaRepository _citaRepository;

        public MedicoService(
            IMedicoRepository medicoRepository,
            ICitaRepository citaRepository)
        {
            _medicoRepository = medicoRepository;
            _citaRepository = citaRepository;
        }

        public List<Medico> ObtenerTodos()
        {
            return _medicoRepository.Leer();
        }

        public Medico? ObtenerPorId(string id)
        {
            return _medicoRepository.Leer()
                .FirstOrDefault(m => m.Id == id);
        }

        public void Crear(Medico medico)
        {
            var medicos = _medicoRepository.Leer();

            medico.Id = GenerarSiguienteId(medicos);
            medicos.Add(medico);

            _medicoRepository.Guardar(medicos);
        }

        public bool Actualizar(Medico medico)
        {
            var medicos = _medicoRepository.Leer();
            var medicoExistente = medicos.FirstOrDefault(m => m.Id == medico.Id);

            if (medicoExistente == null)
            {
                return false;
            }

            medicoExistente.Nombre = medico.Nombre;
            medicoExistente.Apellido = medico.Apellido;
            medicoExistente.Especialidad = medico.Especialidad;
            medicoExistente.NumeroLicencia = medico.NumeroLicencia;

            _medicoRepository.Guardar(medicos);

            return true;
        }

        public bool Eliminar(string id)
        {
            var medicos = _medicoRepository.Leer();
            var medico = medicos.FirstOrDefault(m => m.Id == id);

            if (medico == null)
            {
                return false;
            }

            medicos.Remove(medico);
            _medicoRepository.Guardar(medicos);

            var citas = _citaRepository.Leer();
            var citasActualizadas = citas
                .Where(c => c.MedicoId != id)
                .ToList();

            _citaRepository.Guardar(citasActualizadas);

            return true;
        }

        private static string GenerarSiguienteId(List<Medico> medicos)
        {
            var ultimoNumero = medicos
                .Select(m => m.Id)
                .Where(id => !string.IsNullOrWhiteSpace(id) && id.StartsWith("M"))
                .Select(id => int.TryParse(id[1..], out var numero) ? numero : 0)
                .DefaultIfEmpty(0)
                .Max();

            return $"M{ultimoNumero + 1}";
        }
    }
}
