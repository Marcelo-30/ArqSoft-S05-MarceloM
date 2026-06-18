using CitasApp.Application.Services;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiCors", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IPacienteRepository>(serviceProvider =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var ruta = ObtenerRutaArchivoData(env, "pacientes.json");
    return new JsonPacienteRepository(ruta);
});

builder.Services.AddSingleton<IMedicoRepository>(serviceProvider =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var ruta = ObtenerRutaArchivoData(env, "medicos.json");
    return new JsonMedicoRepository(ruta);
});

builder.Services.AddSingleton<ICitaRepository>(serviceProvider =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var ruta = ObtenerRutaArchivoData(env, "citas.json");
    return new JsonCitaRepository(ruta);
});

builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<CitaService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseCors("ApiCors");
app.UseAuthorization();
app.MapControllers();

app.Run();

static string ObtenerRutaArchivoData(IWebHostEnvironment env, string nombreArchivo)
{
    var rutaCompartidaConWeb = Path.GetFullPath(
        Path.Combine(env.ContentRootPath, "..", "CitasApp.Web", "Data", nombreArchivo));

    if (File.Exists(rutaCompartidaConWeb))
    {
        return rutaCompartidaConWeb;
    }

    var rutaLocalApi = Path.Combine(env.ContentRootPath, "Data", nombreArchivo);
    Directory.CreateDirectory(Path.GetDirectoryName(rutaLocalApi)!);

    if (!File.Exists(rutaLocalApi))
    {
        File.WriteAllText(rutaLocalApi, "[]");
    }

    return rutaLocalApi;
}
