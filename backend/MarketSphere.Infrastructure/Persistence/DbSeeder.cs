using Microsoft.EntityFrameworkCore;
using MarketSphere.Infrastructure.Persistence.Seeders;

namespace MarketSphere.Infrastructure.Persistence;

public sealed class DbSeeder
{
    private readonly MarketSphereDbContext _db;
    private readonly SecuritySeeder _securitySeeder;
    private readonly ReferenceDataSeeder _referenceDataSeeder;
    private readonly OrganizationSeeder _organizationSeeder;
    private readonly CrmSeeder _crmSeeder;
    private readonly ProductPricingSeeder _productPricingSeeder;
    private readonly ProcurementInventorySeeder _procurementInventorySeeder;
    private readonly NumberSequenceSeeder _numberSequenceSeeder;
    private readonly SystemSettingSeeder _systemSettingSeeder;
    private readonly KpiInfrastructureSeeder _kpiInfrastructureSeeder;
    private readonly DemoDataSeeder _demoDataSeeder;

    public DbSeeder(
        MarketSphereDbContext db,
        SecuritySeeder securitySeeder,
        ReferenceDataSeeder referenceDataSeeder,
        OrganizationSeeder organizationSeeder,
        CrmSeeder crmSeeder,
        ProductPricingSeeder productPricingSeeder,
        ProcurementInventorySeeder procurementInventorySeeder,
        NumberSequenceSeeder numberSequenceSeeder,
        SystemSettingSeeder systemSettingSeeder,
        KpiInfrastructureSeeder kpiInfrastructureSeeder,
        DemoDataSeeder demoDataSeeder)
    {
        _db = db;
        _securitySeeder = securitySeeder;
        _referenceDataSeeder = referenceDataSeeder;
        _organizationSeeder = organizationSeeder;
        _crmSeeder = crmSeeder;
        _productPricingSeeder = productPricingSeeder;
        _procurementInventorySeeder = procurementInventorySeeder;
        _numberSequenceSeeder = numberSequenceSeeder;
        _systemSettingSeeder = systemSettingSeeder;
        _kpiInfrastructureSeeder = kpiInfrastructureSeeder;
        _demoDataSeeder = demoDataSeeder;
    }

    public async Task MigrateAndSeedAsync(CancellationToken cancellationToken = default)
    {
        await _db.Database.MigrateAsync(cancellationToken);
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        await _securitySeeder.SeedAsync(cancellationToken);
        await _referenceDataSeeder.SeedAsync(cancellationToken);
        await _organizationSeeder.SeedAsync(cancellationToken);
        await _crmSeeder.SeedAsync(cancellationToken);
        await _productPricingSeeder.SeedAsync(cancellationToken);
        await _procurementInventorySeeder.SeedAsync(cancellationToken);
        await _numberSequenceSeeder.SeedAsync(cancellationToken);
        await _systemSettingSeeder.SeedAsync(cancellationToken);
        await _kpiInfrastructureSeeder.SeedAsync(cancellationToken);
        await _demoDataSeeder.SeedAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
