namespace CitasApp.Infrastructure.Identity
{
    public sealed record UsuarioIdentityDto(
        string Id,
        string Email,
        string? MedicoId,
        IReadOnlyList<string> Roles,
        bool Bloqueado);

    public sealed record CrearUsuarioIdentityRequest(
        string Email,
        string Password,
        string Rol,
        string? MedicoId);

    public sealed record CrearUsuarioIdentityResult(
        bool Succeeded,
        UsuarioIdentityDto? Usuario,
        IReadOnlyList<string> Errors);
}
