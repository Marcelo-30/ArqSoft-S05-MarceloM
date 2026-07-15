using Microsoft.Extensions.DependencyInjection;

namespace CitasApp.Infrastructure.Identity
{
    public static class IdentityInitializationExtensions
    {
        public static async Task InitializeCitasAppIdentityAsync(
            this IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
            IdentitySeeder seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
            await seeder.SeedAsync(cancellationToken);
        }
    }
}
