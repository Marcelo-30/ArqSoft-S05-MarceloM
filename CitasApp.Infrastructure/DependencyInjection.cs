using CitasApp.Application.Services;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Persistence;
using CitasApp.Infrastructure.Repositories;
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
