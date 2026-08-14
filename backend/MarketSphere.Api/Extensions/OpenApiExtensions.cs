using Microsoft.OpenApi.Models;

namespace MarketSphere.Api.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddApiOpenApi(
        this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type =>
                type.FullName?.Replace("+", ".") ?? type.Name);

            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "MarketSphere ConvertX API",
                    Version = "v1",
                    Description =
                        "Localhost API for the MarketSphere ConvertX course project."
                });

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Enter the JWT access token without adding the Bearer prefix."
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        });

        return services;
    }
}
