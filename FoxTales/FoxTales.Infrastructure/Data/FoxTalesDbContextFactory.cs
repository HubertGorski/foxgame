using DotNetEnv;
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

        Env.Load(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.FullName, ".env"));
        var connectionString = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
            ? config.GetValue<string>("ConnectionStrings:DefaultConnection")
            : Environment.GetEnvironmentVariable("ConnectionStringLocal");

        var optionsBuilder = new DbContextOptionsBuilder<FoxTalesDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new FoxTalesDbContext(optionsBuilder.Options);
    }
}
