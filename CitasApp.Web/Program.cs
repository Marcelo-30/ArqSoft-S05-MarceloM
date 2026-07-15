using CitasApp.Application.Services;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Persistence;
using CitasApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración MVC
builder.Services.AddControllersWithViews();

// Obtener la cadena de conexión de appsettings.json
string connectionString =
    builder.Configuration.GetConnectionString("PostgreSql")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'PostgreSql'.");

// Registrar Entity Framework Core con PostgreSQL
builder.Services.AddDbContext<CitasAppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Registrar los repositorios PostgreSQL
builder.Services.AddScoped<
    IPacienteRepository,
    PostgreSqlPacienteRepository>();

builder.Services.AddScoped<
    IMedicoRepository,
    PostgreSqlMedicoRepository>();

builder.Services.AddScoped<
    ICitaRepository,
    PostgreSqlCitaRepository>();

// Registrar los servicios de aplicación
builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<CitaService>();

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();