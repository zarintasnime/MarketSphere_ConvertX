using MarketSphere.Api.Contracts;
using MarketSphere.Api.Middleware;
using MarketSphere.Infrastructure.Persistence;

namespace MarketSphere.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestContextMiddleware>();
        app.UseMiddleware<AuditContextMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "MarketSphere ConvertX API v1");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "MarketSphere ConvertX API";
                options.DisplayRequestDuration();
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("LocalClients");
        app.UseAuthentication();
        app.UseMiddleware<IdempotencyMiddleware>();
        app.UseAuthorization();

        return app;
    }

    public static IEndpointRouteBuilder MapApiFoundation(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
                "/api/health",
                () => Results.Ok(
                    new HealthResponse(
                        "Healthy",
                        "MarketSphere.Api",
                        DateTime.UtcNow)))
            .AllowAnonymous()
            .WithName("GetApiHealth");

        endpoints.MapOpenApi("/openapi/{documentName}.json");

        return endpoints;
    }

    public static async Task ApplyDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
        await seeder.MigrateAndSeedAsync();
    }
}
