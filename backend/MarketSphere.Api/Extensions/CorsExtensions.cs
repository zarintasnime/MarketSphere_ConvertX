namespace MarketSphere.Api.Extensions;

public static class CorsExtensions
{
    public const string LocalClientsPolicy =
        "LocalClients";

    public static IServiceCollection AddLocalCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var origins =
            configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>()
            ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy(
                LocalClientsPolicy,
                policy =>
                {
                    policy
                        .WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        return services;
    }
}
