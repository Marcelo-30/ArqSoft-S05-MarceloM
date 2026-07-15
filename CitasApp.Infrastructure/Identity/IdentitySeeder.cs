using CitasApp.Application.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CitasApp.Infrastructure.Identity
{
    public sealed class IdentitySeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentitySeedOptions _options;

        public IdentitySeeder(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IOptions<IdentitySeedOptions> options)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _options = options.Value;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
            {
                return;
            }

            foreach (string role in RolesAplicacion.Todos)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!await _roleManager.RoleExistsAsync(role))
                {
                    EnsureSucceeded(await _roleManager.CreateAsync(new IdentityRole(role)));
                }
            }

            string email = _options.Admin.Email.Trim();
            string password = _options.Admin.Password;

            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "IdentitySeed:Admin requiere Email y Password cuando se configura un administrador inicial.");
            }

            ApplicationUser? admin = await _userManager.FindByEmailAsync(email);
            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                EnsureSucceeded(await _userManager.CreateAsync(admin, password));
            }

            if (!await _userManager.IsInRoleAsync(admin, RolesAplicacion.Administrador))
            {
                EnsureSucceeded(await _userManager.AddToRoleAsync(admin, RolesAplicacion.Administrador));
            }
        }

        private static void EnsureSucceeded(IdentityResult result)
        {
            if (result.Succeeded)
            {
                return;
            }

            string errors = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"No fue posible inicializar Identity: {errors}");
        }
    }
}
