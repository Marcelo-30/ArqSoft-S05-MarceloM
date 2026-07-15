using CitasApp.Application.Security;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Identity;
using CitasApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace CitasApp.Tests;

public sealed class CitasAppApiFactory : WebApplicationFactory<Program>
{
    public const string AdminEmail = "admin.tests@citasapp.local";
    public const string AdminPassword = "Admin.Tests#2026";
    public const string RecepcionistaEmail = "recepcion.tests@citasapp.local";
    public const string RecepcionistaPassword = "Recepcion.Tests#2026";
    public const string MedicoEmail = "medico.tests@citasapp.local";
    public const string MedicoPassword = "Medico.Tests#2026";
    public const string MedicoId = "medico-integration-tests";

    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private readonly Dictionary<string, string?> _originalEnvironment = new();
    private bool _initialized;

    public CitasAppApiFactory()
    {
        SetTestEnvironment("ASPNETCORE_ENVIRONMENT", "Development");
        SetTestEnvironment("ConnectionStrings__PostgreSql", "Host=integration-tests");
        SetTestEnvironment("Jwt__Issuer", "CitasApp.Tests");
        SetTestEnvironment("Jwt__Audience", "CitasApp.Tests.Client");
        SetTestEnvironment("Jwt__Key", "tests-only-signing-key-with-more-than-32-bytes");
        SetTestEnvironment("Jwt__ExpirationMinutes", "30");
        SetTestEnvironment("IdentitySeed__Enabled", "false");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSql"] = "Host=integration-tests",
                ["Jwt:Issuer"] = "CitasApp.Tests",
                ["Jwt:Audience"] = "CitasApp.Tests.Client",
                ["Jwt:Key"] = "tests-only-signing-key-with-more-than-32-bytes",
                ["Jwt:ExpirationMinutes"] = "30",
                ["IdentitySeed:Enabled"] = "false"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<CitasAppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<CitasAppDbContext>>();
            services.AddDbContext<CitasAppDbContext>(options => options.UseSqlite(_connection));
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        await _initializationLock.WaitAsync();
        try
        {
            if (_initialized)
            {
                return;
            }

            await _connection.OpenAsync();
            _ = CreateClient();

            using IServiceScope scope = Services.CreateScope();
            IServiceProvider services = scope.ServiceProvider;
            CitasAppDbContext context = services.GetRequiredService<CitasAppDbContext>();
            await context.Database.EnsureCreatedAsync();

            RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            IdentitySeeder seeder = new(
                roleManager,
                userManager,
                Options.Create(new IdentitySeedOptions
                {
                    Enabled = true,
                    Admin = new InitialAdminOptions
                    {
                        Email = AdminEmail,
                        Password = AdminPassword
                    }
                }));

            await seeder.SeedAsync();
            await seeder.SeedAsync();

            if (!await context.Medicos.AnyAsync(medico => medico.Id == MedicoId))
            {
                context.Medicos.Add(new Medico
                {
                    Id = MedicoId,
                    Nombre = "Elena",
                    Apellido = "Pruebas",
                    Especialidad = "Medicina general",
                    NumeroLicencia = "TEST-IDENTITY-001"
                });
                await context.SaveChangesAsync();
            }

            await CreateUserAsync(
                userManager,
                RecepcionistaEmail,
                RecepcionistaPassword,
                RolesAplicacion.Recepcionista);
            await CreateUserAsync(
                userManager,
                MedicoEmail,
                MedicoPassword,
                RolesAplicacion.Medico,
                MedicoId);

            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _initializationLock.Dispose();
            _connection.Dispose();
            foreach ((string key, string? value) in _originalEnvironment)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }

        base.Dispose(disposing);
    }

    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role,
        string? medicoId = null)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return;
        }

        ApplicationUser user = new()
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            MedicoId = medicoId
        };

        IdentityResult created = await userManager.CreateAsync(user, password);
        Assert.True(created.Succeeded, string.Join("; ", created.Errors.Select(error => error.Description)));

        IdentityResult assigned = await userManager.AddToRoleAsync(user, role);
        Assert.True(assigned.Succeeded, string.Join("; ", assigned.Errors.Select(error => error.Description)));
    }

    private void SetTestEnvironment(string key, string value)
    {
        _originalEnvironment[key] = Environment.GetEnvironmentVariable(key);
        Environment.SetEnvironmentVariable(key, value);
    }
}
