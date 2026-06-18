using Microsoft.AspNetCore.Mvc;
using CitasApp.Application.Services;
using CitasApp.Domain.Models;

namespace CitasApp.Web.Controllers
{
    public class MedicoController : Controller
    {
        private readonly MedicoService _medicoService;

        public MedicoController(MedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        public IActionResult Index()
        {
            var medicos = _medicoService.ObtenerTodos();
            return View(medicos);
        }

        public IActionResult Detalle(string id)
        {
            var medico = _medicoService.ObtenerPorId(id);

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
            _medicoService.Crear(medico);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(string id)
        {
            var medico = _medicoService.ObtenerPorId(id);

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
            var actualizado = _medicoService.Actualizar(medico);

            if (!actualizado)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(string id)
        {
            var eliminado = _medicoService.Eliminar(id);

            if (!eliminado)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }
    }
}
