using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Infrastructure.Persistence.Seeders;

public sealed class ReferenceDataSeeder
{
    private readonly MarketSphereDbContext _db;
    private readonly IDateTimeProvider _clock;

    public ReferenceDataSeeder(
        MarketSphereDbContext db,
        IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var designations = new[]
        {
            new DesignationSeed("GM", "General Manager", 10, false),
            new DesignationSeed("CRM_MANAGER", "CRM Manager", 20, false),
            new DesignationSeed("MARKETING_MANAGER", "Marketing Manager", 20, false),
            new DesignationSeed("SALES_MANAGER", "Sales Manager", 20, false),
            new DesignationSeed("SO", "Sales Officer", 40, true),
            new DesignationSeed("MT_EXECUTIVE", "MT Executive", 40, true),
            new DesignationSeed("BP", "Business Promoter", 50, true),
            new DesignationSeed("MERCHANDISER", "Merchandiser", 50, true),
            new DesignationSeed("PROCUREMENT", "Procurement Officer", 40, false),
            new DesignationSeed("WAREHOUSE", "Warehouse Officer", 40, false),
            new DesignationSeed("DELIVERY", "Delivery Officer", 50, true)
        };

        foreach (var seed in designations)
        {
            var designation = await _db.Designations
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x => x.DesignationCode == seed.Code,
                    cancellationToken);

            if (designation is null)
            {
                designation = new Designation
                {
                    DesignationCode = seed.Code,
                    DesignationName = seed.Name,
                    HierarchyLevel = seed.Level,
                    IsFieldRole = seed.IsFieldRole,
                    IsActive = true,
                    CreatedAt = _clock.UtcNow
                };

                await _db.Designations.AddAsync(
                    designation,
                    cancellationToken);
            }
            else
            {
                designation.DesignationName = seed.Name;
                designation.HierarchyLevel = seed.Level;
                designation.IsFieldRole = seed.IsFieldRole;
                designation.IsActive = true;
                designation.IsDeleted = false;
                designation.DeletedAt = null;
                designation.DeletedByUserID = null;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private sealed record DesignationSeed(
        string Code,
        string Name,
        int Level,
        bool IsFieldRole);
}
