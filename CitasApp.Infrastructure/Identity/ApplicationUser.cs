using Microsoft.AspNetCore.Identity;

namespace CitasApp.Infrastructure.Identity
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string? MedicoId { get; set; }
    }
}
