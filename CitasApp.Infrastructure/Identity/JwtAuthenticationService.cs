using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace CitasApp.Infrastructure.Identity
{
    public sealed class JwtAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtOptions _options;

        public JwtAuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtOptions> options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _options = options.Value;
        }

        public async Task<JwtAuthenticationResult> AuthenticateAsync(
            string email,
            string password)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email.Trim());
            if (user is null)
            {
                return Failed();
            }

            Microsoft.AspNetCore.Identity.SignInResult passwordResult =
                await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (!passwordResult.Succeeded)
            {
                return Failed();
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset expires = now.AddMinutes(_options.ExpirationMinutes);

            List<Claim> claims =
            [
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.Email ?? string.Empty)
            ];

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            if (!string.IsNullOrWhiteSpace(user.MedicoId))
            {
                claims.Add(new Claim("medico_id", user.MedicoId));
            }

            SigningCredentials credentials = new(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
                SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: expires.UtcDateTime,
                signingCredentials: credentials);

            UsuarioIdentityDto usuario = new(
                user.Id,
                user.Email ?? string.Empty,
                user.MedicoId,
                roles.ToList(),
                false);

            return new JwtAuthenticationResult(
                true,
                new JwtSecurityTokenHandler().WriteToken(token),
                expires,
                usuario);
        }

        private static JwtAuthenticationResult Failed()
        {
            return new JwtAuthenticationResult(false, null, null, null);
        }
    }
}
