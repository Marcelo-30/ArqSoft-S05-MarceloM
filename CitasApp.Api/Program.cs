using System.Security.Claims;
using System.Text;
using CitasApp.Application.Strategies.Calculadora;
using CitasApp.Infrastructure;
using CitasApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCitasAppInfrastructure(builder.Configuration);

JwtOptions jwtOptions = JwtOptions.GetValidated(builder.Configuration);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

string[] allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiCors", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            throw new InvalidOperationException(
                "Cors:AllowedOrigins debe configurarse fuera del entorno Development.");
        }
    });
});

builder.Services.AddSingleton<IOperacionCalculadora, SumaOperacionCalculadora>();
builder.Services.AddSingleton<IOperacionCalculadora, RestaOperacionCalculadora>();
builder.Services.AddSingleton<IOperacionCalculadora, MultiplicacionOperacionCalculadora>();
builder.Services.AddSingleton<IOperacionCalculadora, DivisionOperacionCalculadora>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("ApiCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.Services.InitializeCitasAppIdentityAsync();
app.Run();

public partial class Program;
