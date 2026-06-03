using Microsoft.AspNetCore.Mvc;
using CitasApp.Models;
using CitasApp.Services;

namespace CitasApp.Controllers
{
    public class MedicoController : Controller
    {
        private readonly JsonFileService<Medico> _medicoService;

        public MedicoController(IWebHostEnvironment env)
        {
            var ruta = Path.Combine(env.ContentRootPath, "Data", "medicos.json");
            _medicoService = new JsonFileService<Medico>(ruta);
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
        public IActionResult Crear(Medico medico)
        {
            var medicos = _medicoService.Leer();

            medico.Id = "M" + (medicos.Count + 1);

            medicos.Add(medico);

            _medicoService.Guardar(medicos);

            return RedirectToAction("Index");
        }
    }
}
