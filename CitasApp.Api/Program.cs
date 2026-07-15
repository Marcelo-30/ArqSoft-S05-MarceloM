using CitasApp.Application.Services;
using CitasApp.Application.Strategies.Calculadora;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Persistence;
using CitasApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controladores de la API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiCors", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Obtener la cadena de conexión
string connectionString =
    builder.Configuration.GetConnectionString("PostgreSql")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'PostgreSql'.");

// Configurar Entity Framework Core con PostgreSQL
builder.Services.AddDbContext<CitasAppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Repositorios PostgreSQL
builder.Services.AddScoped<
    IPacienteRepository,
    PostgreSqlPacienteRepository>();

builder.Services.AddScoped<
    IMedicoRepository,
    PostgreSqlMedicoRepository>();

builder.Services.AddScoped<
    ICitaRepository,
    PostgreSqlCitaRepository>();

// Estrategias de la calculadora
builder.Services.AddSingleton<
    IOperacionCalculadora,
    SumaOperacionCalculadora>();

builder.Services.AddSingleton<
    IOperacionCalculadora,
    RestaOperacionCalculadora>();

builder.Services.AddSingleton<
    IOperacionCalculadora,
    MultiplicacionOperacionCalculadora>();

builder.Services.AddSingleton<
    IOperacionCalculadora,
    DivisionOperacionCalculadora>();

// Servicios de aplicación
builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<CitaService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("ApiCors");

app.UseAuthorization();

app.MapControllers();

app.Run();