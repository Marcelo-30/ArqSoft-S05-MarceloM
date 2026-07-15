using CitasApp.Api.Dtos;
using CitasApp.Application.Security;
using CitasApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private const string LoginError = "No fue posible iniciar sesion con las credenciales proporcionadas.";
        private readonly JwtAuthenticationService _authenticationService;
        private readonly UsuarioIdentityService _usuarioService;

        public AuthController(
            JwtAuthenticationService authenticationService,
            UsuarioIdentityService usuarioService)
        {
            _authenticationService = authenticationService;
            _usuarioService = usuarioService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
        {
            JwtAuthenticationResult result = await _authenticationService.AuthenticateAsync(
                request.Email,
                request.Password);

            if (!result.Succeeded || result.Usuario is null ||
                result.Token is null || result.ExpiresAtUtc is null)
            {
                return Unauthorized(new { mensaje = LoginError });
            }

            return Ok(MapAuthentication(result));
        }

        [Authorize(Roles = RolesAplicacion.Administrador)]
        [HttpGet("users")]
        public async Task<ActionResult<IReadOnlyList<AuthenticatedUserDto>>> ObtenerUsuarios(
            CancellationToken cancellationToken)
        {
            IReadOnlyList<UsuarioIdentityDto> users =
                await _usuarioService.ObtenerTodosAsync(cancellationToken);
            return Ok(users.Select(MapUser).ToList());
        }

        [Authorize(Roles = RolesAplicacion.Administrador)]
        [HttpPost("users")]
        public async Task<ActionResult<AuthenticatedUserDto>> CrearUsuario(
            CrearUsuarioRequestDto request,
            CancellationToken cancellationToken)
        {
            CrearUsuarioIdentityResult result = await _usuarioService.CrearAsync(
                new CrearUsuarioIdentityRequest(
                    request.Email,
                    request.Password,
                    request.Rol,
                    request.MedicoId),
                cancellationToken);

            if (!result.Succeeded || result.Usuario is null)
            {
                object error = new { errores = result.Errors };
                return result.Conflict ? Conflict(error) : BadRequest(error);
            }

            AuthenticatedUserDto response = MapUser(result.Usuario);
            return Created("/api/auth/users", response);
        }

        private static AuthResponseDto MapAuthentication(JwtAuthenticationResult result)
        {
            return new AuthResponseDto(
                result.Token!,
                result.ExpiresAtUtc!.Value,
                MapUser(result.Usuario!));
        }

        private static AuthenticatedUserDto MapUser(UsuarioIdentityDto user)
        {
            return new AuthenticatedUserDto(
                user.Id,
                user.Email,
                user.MedicoId,
                user.Roles);
        }
    }
}
