using MarketSphere.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MarketSphere.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions =
            configuration.GetSection(
                JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration is missing.");

        if (Encoding.UTF8.GetByteCount(
                jwtOptions.SigningKey) < 32)
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey must contain at least 32 bytes.");
        }

        services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = true;
                options.SaveToken = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtOptions.SigningKey)),
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
            });

        return services;
    }
}
