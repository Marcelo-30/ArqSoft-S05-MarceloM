using CitasApp.Application.Services;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var usarPacientesEnMemoria = false;

if (usarPacientesEnMemoria)
{
    builder.Services.AddSingleton<IPacienteRepository, MemoriaPacienteRepository>();
}
else
{
    builder.Services.AddSingleton<IPacienteRepository>(serviceProvider =>
    {
        var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var ruta = Path.Combine(env.ContentRootPath, "Data", "pacientes.json");
        return new JsonPacienteRepository(ruta);
    });
}

builder.Services.AddSingleton<IMedicoRepository>(serviceProvider =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var ruta = Path.Combine(env.ContentRootPath, "Data", "medicos.json");
    return new JsonMedicoRepository(ruta);
});

builder.Services.AddSingleton<ICitaRepository>(serviceProvider =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var ruta = Path.Combine(env.ContentRootPath, "Data", "citas.json");
    return new JsonCitaRepository(ruta);
});

builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<CitaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
