using CitasApp.Infrastructure.Identity;
using CitasApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Web.Controllers
{
    public sealed class CuentaController : Controller
    {
        private const string LoginError = "No fue posible iniciar sesion con las credenciales proporcionadas.";
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CuentaController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult IniciarSesion(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirigirLocal(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new IniciarSesionViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarSesion(
            IniciarSesionViewModel model,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                model.Email.Trim(),
                model.Password,
                model.Recordarme,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return RedirigirLocal(returnUrl);
            }

            ModelState.AddModelError(string.Empty, LoginError);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(IniciarSesion));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccesoDenegado() => View();

        private IActionResult RedirigirLocal(string? returnUrl)
        {
            return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? LocalRedirect(returnUrl)
                : RedirectToAction("Index", "Home");
        }
    }
}
