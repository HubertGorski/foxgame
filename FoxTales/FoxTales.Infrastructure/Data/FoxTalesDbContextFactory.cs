using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FoxTales.Infrastructure.Data;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<FoxTalesDbContext>
{
    public FoxTalesDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FoxTales.Api"))
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets("d47d81f4-8cd9-4abc-8ab4-d31e7206d646")
            .Build();

        var connectionString = config.GetConnectionString("Default");

        var optionsBuilder = new DbContextOptionsBuilder<FoxTalesDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new FoxTalesDbContext(optionsBuilder.Options);
    }
}
