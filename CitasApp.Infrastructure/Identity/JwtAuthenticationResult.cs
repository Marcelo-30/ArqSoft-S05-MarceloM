namespace CitasApp.Infrastructure.Identity
{
    public sealed record JwtAuthenticationResult(
        bool Succeeded,
        string? Token,
        DateTimeOffset? ExpiresAtUtc,
        UsuarioIdentityDto? Usuario);
}
