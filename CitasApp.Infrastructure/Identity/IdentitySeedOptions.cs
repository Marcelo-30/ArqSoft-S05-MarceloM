namespace CitasApp.Infrastructure.Identity
{
    public sealed class IdentitySeedOptions
    {
        public const string SectionName = "IdentitySeed";

        public bool Enabled { get; set; }

        public InitialAdminOptions Admin { get; set; } = new();
    }

    public sealed class InitialAdminOptions
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
