using CitasApp.Application.Security;
using CitasApp.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Infrastructure.Identity
{
    public sealed class UsuarioIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMedicoRepository _medicoRepository;

        public UsuarioIdentityService(
            UserManager<ApplicationUser> userManager,
            IMedicoRepository medicoRepository)
        {
            _userManager = userManager;
            _medicoRepository = medicoRepository;
        }

        public async Task<IReadOnlyList<UsuarioIdentityDto>> ObtenerTodosAsync(
            CancellationToken cancellationToken = default)
        {
            List<ApplicationUser> users = await _userManager.Users
                .AsNoTracking()
                .OrderBy(user => user.Email)
                .ToListAsync(cancellationToken);

            List<UsuarioIdentityDto> result = [];
            foreach (ApplicationUser user in users)
            {
                cancellationToken.ThrowIfCancellationRequested();
                result.Add(await MapAsync(user));
            }

            return result;
        }

        public async Task<CrearUsuarioIdentityResult> CrearAsync(
            CrearUsuarioIdentityRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            cancellationToken.ThrowIfCancellationRequested();

            string email = request.Email.Trim();
            string rol = request.Rol.Trim();
            string? medicoId = string.IsNullOrWhiteSpace(request.MedicoId)
                ? null
                : request.MedicoId.Trim();

            List<string> errors = [];
            bool conflict = false;
            if (!RolesAplicacion.Todos.Contains(rol))
            {
                errors.Add("El rol seleccionado no es valido.");
            }

            if (rol == RolesAplicacion.Medico)
            {
                if (medicoId is null ||
                    await _medicoRepository.ObtenerPorIdAsync(medicoId, cancellationToken) is null)
                {
                    errors.Add("Un usuario Medico debe vincularse con un medico existente.");
                }
                else if (await _userManager.Users.AnyAsync(
                    user => user.MedicoId == medicoId,
                    cancellationToken))
                {
                    errors.Add("El medico seleccionado ya tiene un usuario vinculado.");
                    conflict = true;
                }
            }
            else
            {
                medicoId = null;
            }

            if (errors.Count > 0)
            {
                return new CrearUsuarioIdentityResult(false, conflict, null, errors);
            }

            if (await _userManager.FindByEmailAsync(email) is not null)
            {
                return new CrearUsuarioIdentityResult(
                    false,
                    true,
                    null,
                    ["Ya existe un usuario con ese correo."]);
            }

            ApplicationUser user = new()
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                MedicoId = medicoId
            };

            IdentityResult createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                return Failed(createResult);
            }

            IdentityResult roleResult = await _userManager.AddToRoleAsync(user, rol);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return Failed(roleResult);
            }

            return new CrearUsuarioIdentityResult(
                true,
                false,
                await MapAsync(user),
                Array.Empty<string>());
        }

        private async Task<UsuarioIdentityDto> MapAsync(ApplicationUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            return new UsuarioIdentityDto(
                user.Id,
                user.Email ?? string.Empty,
                user.MedicoId,
                roles.ToList(),
                user.LockoutEnd > DateTimeOffset.UtcNow);
        }

        private static CrearUsuarioIdentityResult Failed(IdentityResult result)
        {
            return new CrearUsuarioIdentityResult(
                false,
                result.Errors.Any(error => error.Code.StartsWith("Duplicate", StringComparison.Ordinal)),
                null,
                result.Errors.Select(error => error.Description).ToList());
        }
    }
}
