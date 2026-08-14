using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Infrastructure.FileStorage;
using MarketSphere.Infrastructure.Identity;
using MarketSphere.Infrastructure.Persistence;
using MarketSphere.Infrastructure.Persistence.Interceptors;
using MarketSphere.Infrastructure.Persistence.Seeders;
using MarketSphere.Infrastructure.Services;
using MarketSphere.Infrastructure.Reports;

namespace MarketSphere.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "MarketSphereDb")
            ?? throw new InvalidOperationException(
                "Connection string 'MarketSphereDb' was not found.");

        services.Configure<JwtOptions>(
            configuration.GetSection(
                JwtOptions.SectionName));

        services.AddHttpContextAccessor();

        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<StatusHistoryInterceptor>();
        services.AddScoped<AuditLogInterceptor>();

        services.AddDbContext<MarketSphereDbContext>(
            (serviceProvider, options) =>
            {
                options.UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(
                        typeof(MarketSphereDbContext)
                            .Assembly.FullName));

                options.AddInterceptors(
                    serviceProvider.GetRequiredService<
                        SoftDeleteInterceptor>(),
                    serviceProvider.GetRequiredService<
                        AuditableEntityInterceptor>(),
                    serviceProvider.GetRequiredService<
                        StatusHistoryInterceptor>(),
                    serviceProvider.GetRequiredService<
                        AuditLogInterceptor>());
            });

        services.AddScoped<IApplicationDbContext>(
            serviceProvider =>
                serviceProvider.GetRequiredService<
                    MarketSphereDbContext>());

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<INumberSequenceService, NumberSequenceService>();
        services.AddScoped<MarketSphere.Application.Common.Interfaces.ISystemCheckService, MarketSphere.Infrastructure.Services.SystemCheckService>();
        services.AddScoped<IAnalyticsQueryService, AnalyticsQueryService>();
        services.AddScoped<IKpiProjectionService, KpiProjectionService>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();

        services.AddScoped<SecuritySeeder>();
        services.AddScoped<ReferenceDataSeeder>();
        services.AddScoped<OrganizationSeeder>();
        services.AddScoped<CrmSeeder>();
        services.AddScoped<ProductPricingSeeder>();
        services.AddScoped<ProcurementInventorySeeder>();
        services.AddScoped<NumberSequenceSeeder>();
        services.AddScoped<SystemSettingSeeder>();
        services.AddScoped<KpiInfrastructureSeeder>();
        services.AddScoped<DemoDataSeeder>();
        services.AddScoped<DbSeeder>();

        return services;
    }
}
