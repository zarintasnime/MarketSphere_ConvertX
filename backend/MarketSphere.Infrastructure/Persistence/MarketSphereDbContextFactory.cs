using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MarketSphere.Infrastructure.Persistence;

public sealed class MarketSphereDbContextFactory :
    IDesignTimeDbContextFactory<MarketSphereDbContext>
{
    public MarketSphereDbContext CreateDbContext(
        string[] args)
    {
        var environment =
            Environment.GetEnvironmentVariable(
                "ASPNETCORE_ENVIRONMENT")
            ?? "Development";

        var currentDirectory =
            Directory.GetCurrentDirectory();

        var candidatePaths = new[]
        {
            Path.Combine(
                currentDirectory,
                "MarketSphere.Api"),
            Path.Combine(
                currentDirectory,
                "..",
                "MarketSphere.Api"),
            currentDirectory
        };

        var basePath = candidatePaths
            .Select(Path.GetFullPath)
            .FirstOrDefault(
                path => File.Exists(
                    Path.Combine(
                        path,
                        "appsettings.json")))
            ?? throw new DirectoryNotFoundException(
                "MarketSphere.Api/appsettings.json could not be located.");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile(
                "appsettings.json",
                optional: false)
            .AddJsonFile(
                $"appsettings.{environment}.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString(
                "MarketSphereDb")
            ?? throw new InvalidOperationException(
                "Connection string 'MarketSphereDb' was not found.");

        var options =
            new DbContextOptionsBuilder<
                MarketSphereDbContext>()
                .UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(
                        typeof(MarketSphereDbContext)
                            .Assembly.FullName))
                .Options;

        return new MarketSphereDbContext(options);
    }
}
