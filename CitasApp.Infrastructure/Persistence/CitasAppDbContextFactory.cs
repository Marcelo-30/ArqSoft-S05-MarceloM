using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CitasApp.Infrastructure.Persistence
{
    public sealed class CitasAppDbContextFactory : IDesignTimeDbContextFactory<CitasAppDbContext>
    {
        public CitasAppDbContext CreateDbContext(string[] args)
        {
            string? connectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__PostgreSql");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Configura ConnectionStrings__PostgreSql para administrar migraciones.");
            }

            DbContextOptions<CitasAppDbContext> options =
                new DbContextOptionsBuilder<CitasAppDbContext>()
                    .UseNpgsql(connectionString)
                    .Options;

            return new CitasAppDbContext(options);
        }
    }
}
