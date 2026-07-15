namespace CitasApp.Api.Dtos
{
    public sealed record AuthResponseDto(
        string Token,
        DateTimeOffset ExpiresAtUtc,
        AuthenticatedUserDto User);

    public sealed record AuthenticatedUserDto(
        string Id,
        string Email,
        string? MedicoId,
        IReadOnlyList<string> Roles);
}
