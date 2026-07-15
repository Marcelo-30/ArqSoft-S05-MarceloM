using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CitasApp.Api.Dtos;
using CitasApp.Application.Security;
using CitasApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CitasApp.Tests;

public sealed class AuthenticationAndAuthorizationTests :
    IClassFixture<CitasAppApiFactory>,
    IAsyncLifetime
{
    private readonly CitasAppApiFactory _factory;

    public AuthenticationAndAuthorizationTests(CitasAppApiFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync()
    {
        return _factory.InitializeDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task IdentitySeeder_CreatesRolesAndAdminOnlyOnce()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        RoleManager<IdentityRole> roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<ApplicationUser> userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        foreach (string role in RolesAplicacion.Todos)
        {
            Assert.True(await roleManager.RoleExistsAsync(role));
        }

        ApplicationUser admin = Assert.IsType<ApplicationUser>(
            await userManager.FindByEmailAsync(CitasAppApiFactory.AdminEmail));
        Assert.True(await userManager.IsInRoleAsync(admin, RolesAplicacion.Administrador));
        Assert.NotEqual(CitasAppApiFactory.AdminPassword, admin.PasswordHash);
        Assert.Equal(
            1,
            userManager.Users.Count(user => user.NormalizedEmail == admin.NormalizedEmail));
    }

    [Fact]
    public async Task Login_WithCorrectPassword_ReturnsTokenUserAndRoles()
    {
        using HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await LoginAsync(
            client,
            CitasAppApiFactory.RecepcionistaEmail,
            CitasAppApiFactory.RecepcionistaPassword);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AuthResponseDto result = Assert.IsType<AuthResponseDto>(
            await response.Content.ReadFromJsonAsync<AuthResponseDto>());
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.True(result.ExpiresAtUtc > DateTimeOffset.UtcNow);
        Assert.Equal(CitasAppApiFactory.RecepcionistaEmail, result.User.Email);
        Assert.Contains(RolesAplicacion.Recepcionista, result.User.Roles);
    }

    [Fact]
    public async Task Login_WithIncorrectPassword_ReturnsGenericUnauthorizedResponse()
    {
        using HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await LoginAsync(
            client,
            CitasAppApiFactory.RecepcionistaEmail,
            "Incorrect.Password#2026");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("No fue posible iniciar sesion", body);
        Assert.DoesNotContain(CitasAppApiFactory.RecepcionistaEmail, body);
    }

    [Fact]
    public async Task Patients_WithoutToken_ReturnsUnauthorized()
    {
        using HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/pacientes");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Patients_WithValidReceptionistToken_ReturnsOk()
    {
        using HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            await GetTokenAsync(
                client,
                CitasAppApiFactory.RecepcionistaEmail,
                CitasAppApiFactory.RecepcionistaPassword));

        HttpResponseMessage response = await client.GetAsync("/api/pacientes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Patients_WithDoctorRole_ReturnsForbidden()
    {
        using HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            await GetTokenAsync(
                client,
                CitasAppApiFactory.MedicoEmail,
                CitasAppApiFactory.MedicoPassword));

        HttpResponseMessage response = await client.GetAsync("/api/pacientes");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static Task<HttpResponseMessage> LoginAsync(
        HttpClient client,
        string email,
        string password)
    {
        return client.PostAsJsonAsync("/api/auth/login", new { email, password });
    }

    private static async Task<string> GetTokenAsync(
        HttpClient client,
        string email,
        string password)
    {
        HttpResponseMessage response = await LoginAsync(client, email, password);
        response.EnsureSuccessStatusCode();
        AuthResponseDto result = Assert.IsType<AuthResponseDto>(
            await response.Content.ReadFromJsonAsync<AuthResponseDto>());
        return result.Token;
    }
}
