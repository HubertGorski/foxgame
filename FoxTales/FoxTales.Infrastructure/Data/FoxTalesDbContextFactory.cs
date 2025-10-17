using FoxTales.Infrastructure.Configuration;
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
            .AddEnvironmentVariables()
            .Build();

        var connectionString = ConnectionStringProvider.GetConnectionString(config);
        var optionsBuilder = new DbContextOptionsBuilder<FoxTalesDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new FoxTalesDbContext(optionsBuilder.Options);
    }
}
