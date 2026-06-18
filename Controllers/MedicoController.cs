using Microsoft.AspNetCore.Mvc;
using CitasApp.Models;
using CitasApp.Services;

namespace CitasApp.Controllers
{
    public class MedicoController : Controller
    {
        private readonly IJsonFileService<Medico> _medicoService;
        private readonly IJsonFileService<Cita> _citaService;

        public MedicoController(
            IJsonFileService<Medico> medicoService,
            IJsonFileService<Cita> citaService)
        {
            _medicoService = medicoService;
            _citaService = citaService;
        }

        public IActionResult Index()
        {
            var medicos = _medicoService.Leer();
            return View(medicos);
        }

        public IActionResult Detalle(string id)
        {
            var medicos = _medicoService.Leer();
            var medico = medicos.FirstOrDefault(m => m.Id == id);

            if (medico == null)
            {
                return NotFound();
            }

            return View(medico);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Medico medico)
        {
            var medicos = _medicoService.Leer();

            medico.Id = GenerarSiguienteId(medicos);
            medicos.Add(medico);

            _medicoService.Guardar(medicos);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(string id)
        {
            var medicos = _medicoService.Leer();
            var medico = medicos.FirstOrDefault(m => m.Id == id);

            if (medico == null)
            {
                return NotFound();
            }

            return View(medico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Medico medico)
        {
            var medicos = _medicoService.Leer();
            var medicoExistente = medicos.FirstOrDefault(m => m.Id == medico.Id);

            if (medicoExistente == null)
            {
                return NotFound();
            }

            medicoExistente.Nombre = medico.Nombre;
            medicoExistente.Apellido = medico.Apellido;
            medicoExistente.Especialidad = medico.Especialidad;
            medicoExistente.NumeroLicencia = medico.NumeroLicencia;

            _medicoService.Guardar(medicos);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(string id)
        {
            var medicos = _medicoService.Leer();
            var medico = medicos.FirstOrDefault(m => m.Id == id);

            if (medico == null)
            {
                return NotFound();
            }

            medicos.Remove(medico);
            _medicoService.Guardar(medicos);

            var citas = _citaService.Leer();
            var citasActualizadas = citas
                .Where(c => c.MedicoId != id)
                .ToList();

            _citaService.Guardar(citasActualizadas);

            return RedirectToAction("Index");
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
