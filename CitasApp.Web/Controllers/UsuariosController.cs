using CitasApp.Application.Security;
using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Identity;
using CitasApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CitasApp.Web.Controllers
{
    [Authorize(Roles = RolesAplicacion.Administrador)]
    public sealed class UsuariosController : Controller
    {
        private readonly UsuarioIdentityService _usuarioService;
        private readonly MedicoService _medicoService;

        public UsuariosController(
            UsuarioIdentityService usuarioService,
            MedicoService medicoService)
        {
            _usuarioService = usuarioService;
            _medicoService = medicoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View(await _usuarioService.ObtenerTodosAsync(cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> Crear(CancellationToken cancellationToken)
        {
            await CargarOpcionesAsync(null, null, cancellationToken);
            return View(new CrearUsuarioViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            CrearUsuarioViewModel model,
            CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                CrearUsuarioIdentityResult result = await _usuarioService.CrearAsync(
                    new CrearUsuarioIdentityRequest(
                        model.Email,
                        model.Password,
                        model.Rol,
                        model.MedicoId),
                    cancellationToken);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (string error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            await CargarOpcionesAsync(model.Rol, model.MedicoId, cancellationToken);
            return View(model);
        }

        private async Task CargarOpcionesAsync(
            string? rol,
            string? medicoId,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Medico> medicos = await _medicoService.ObtenerTodosAsync(cancellationToken);
            ViewBag.Roles = new SelectList(RolesAplicacion.Todos.OrderBy(value => value), rol);
            ViewBag.Medicos = new SelectList(
                medicos.Select(medico => new
                {
                    medico.Id,
                    Nombre = $"{medico.Nombre} {medico.Apellido}".Trim()
                }),
                "Id",
                "Nombre",
                medicoId);
        }
    }
}
