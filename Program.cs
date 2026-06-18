using CitasApp.Models;
using CitasApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IJsonFileService<Paciente>>(serviceProvider =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var ruta = Path.Combine(env.ContentRootPath, "Data", "pacientes.json");
    return new JsonFileService<Paciente>(ruta);
});

builder.Services.AddSingleton<IJsonFileService<Medico>>(serviceProvider =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var ruta = Path.Combine(env.ContentRootPath, "Data", "medicos.json");
    return new JsonFileService<Medico>(ruta);
});

builder.Services.AddSingleton<IJsonFileService<Cita>>(serviceProvider =>
{
    var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var ruta = Path.Combine(env.ContentRootPath, "Data", "citas.json");
    return new JsonFileService<Cita>(ruta);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
