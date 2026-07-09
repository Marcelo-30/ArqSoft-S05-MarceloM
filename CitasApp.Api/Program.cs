using CitasApp.Application.Services;
using CitasApp.Application.Strategies.Calculadora;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Factories;

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
    var factory = new JsonPacienteRepositoryFactory(env.ContentRootPath);
    return factory.Crear();
});

builder.Services.AddSingleton<IMedicoRepository>(serviceProvider =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var factory = new JsonMedicoRepositoryFactory(env.ContentRootPath);
    return factory.Crear();
});

builder.Services.AddSingleton<ICitaRepository>(serviceProvider =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var factory = new JsonCitaRepositoryFactory(env.ContentRootPath);
    return factory.Crear();
});

builder.Services.AddSingleton<IOperacionCalculadora, SumaOperacionCalculadora>();
builder.Services.AddSingleton<IOperacionCalculadora, RestaOperacionCalculadora>();
builder.Services.AddSingleton<IOperacionCalculadora, MultiplicacionOperacionCalculadora>();
builder.Services.AddSingleton<IOperacionCalculadora, DivisionOperacionCalculadora>();

builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<CitaService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("ApiCors");
app.UseAuthorization();
app.MapControllers();

app.Run();
