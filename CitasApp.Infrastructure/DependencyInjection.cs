using CitasApp.Application.Services;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Identity;
using CitasApp.Infrastructure.Persistence;
using CitasApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CitasApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCitasAppInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("PostgreSql");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "No se configuro la cadena de conexion 'ConnectionStrings:PostgreSql'.");
            }

            services.AddDbContext<CitasAppDbContext>(options => options.UseNpgsql(connectionString));

            services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.Password.RequiredLength = 12;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<CitasAppDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.Configure<IdentitySeedOptions>(
                configuration.GetSection(IdentitySeedOptions.SectionName));
            services.AddScoped<IdentitySeeder>();
            services.AddScoped<UsuarioIdentityService>();

            services.AddScoped<IPacienteRepository, PostgreSqlPacienteRepository>();
            services.AddScoped<IMedicoRepository, PostgreSqlMedicoRepository>();
            services.AddScoped<ICitaRepository, PostgreSqlCitaRepository>();

            services.AddScoped<PacienteService>();
            services.AddScoped<MedicoService>();
            services.AddScoped<CitaService>();

            return services;
        }
    }
}
