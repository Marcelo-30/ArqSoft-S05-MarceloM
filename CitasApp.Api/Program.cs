using CitasApp.Application.Strategies.Calculadora;
using CitasApp.Infrastructure;

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

builder.Services.AddCitasAppInfrastructure(builder.Configuration);

builder.Services.AddSingleton<IOperacionCalculadora, SumaOperacionCalculadora>();
builder.Services.AddSingleton<IOperacionCalculadora, RestaOperacionCalculadora>();
builder.Services.AddSingleton<IOperacionCalculadora, MultiplicacionOperacionCalculadora>();
builder.Services.AddSingleton<IOperacionCalculadora, DivisionOperacionCalculadora>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("ApiCors");
app.UseAuthorization();
app.MapControllers();

app.Run();
