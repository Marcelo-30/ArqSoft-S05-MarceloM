using System.Text;
using Microsoft.Extensions.Configuration;

namespace CitasApp.Infrastructure.Identity
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public int ExpirationMinutes { get; set; } = 60;

        public static JwtOptions GetValidated(IConfiguration configuration)
        {
            JwtOptions options = configuration.GetSection(SectionName).Get<JwtOptions>() ?? new();

            if (string.IsNullOrWhiteSpace(options.Issuer) ||
                string.IsNullOrWhiteSpace(options.Audience) ||
                string.IsNullOrWhiteSpace(options.Key))
            {
                throw new InvalidOperationException(
                    "Jwt:Issuer, Jwt:Audience y Jwt:Key deben configurarse mediante User Secrets o variables de entorno.");
            }

            if (Encoding.UTF8.GetByteCount(options.Key) < 32)
            {
                throw new InvalidOperationException("Jwt:Key debe contener al menos 32 bytes.");
            }

            if (options.ExpirationMinutes is < 5 or > 1440)
            {
                throw new InvalidOperationException("Jwt:ExpirationMinutes debe estar entre 5 y 1440.");
            }

            return options;
        }
    }
}
