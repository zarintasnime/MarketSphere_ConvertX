using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MarketSphere.Infrastructure.Persistence.Seeders;

public sealed class OrganizationSeeder
{
    private readonly MarketSphereDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IDateTimeProvider _clock;

    public OrganizationSeeder(
        MarketSphereDbContext db,
        IConfiguration configuration,
        IDateTimeProvider clock)
    {
        _db = db;
        _configuration = configuration;
        _clock = clock;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var company = await SeedCompanyAsync(cancellationToken);
        var branch = await SeedBranchAsync(
            company.CompanyID,
            cancellationToken);

        var dhakaRegion = await SeedRegionAsync(
            company.CompanyID,
            "DHAKA",
            "Dhaka Region",
            cancellationToken);

        var chattogramRegion = await SeedRegionAsync(
            company.CompanyID,
            "CHATTOGRAM",
            "Chattogram Region",
            cancellationToken);

        var dhakaArea = await SeedAreaAsync(
            dhakaRegion.RegionID,
            "DHAKA-METRO",
            "Dhaka Metro Area",
            cancellationToken);

        var chattogramArea = await SeedAreaAsync(
            chattogramRegion.RegionID,
            "CTG-METRO",
            "Chattogram Metro Area",
            cancellationToken);

        var mirpurTerritory = await SeedTerritoryAsync(
            dhakaArea.AreaID,
            "MIRPUR",
            "Mirpur Territory",
            cancellationToken);

        var agrabadTerritory = await SeedTerritoryAsync(
            chattogramArea.AreaID,
            "AGRABAD",
            "Agrabad Territory",
            cancellationToken);

        await SeedRouteAsync(
            mirpurTerritory.TerritoryID,
            "MIRPUR-R01",
            "Mirpur 10 - Pallabi Route",
            DayOfWeek.Sunday,
            cancellationToken);

        await SeedRouteAsync(
            agrabadTerritory.TerritoryID,
            "AGRABAD-R01",
            "Agrabad - Halishahar Route",
            DayOfWeek.Monday,
            cancellationToken);

        await SeedAdminEmployeeAsync(
            branch.BranchID,
            cancellationToken);
    }

    private async Task<Company> SeedCompanyAsync(
        CancellationToken cancellationToken)
    {
        const string code = "MSCX";

        var company = await _db.Companies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.CompanyCode == code,
                cancellationToken);

        if (company is null)
        {
            company = new Company
            {
                CompanyCode = code,
                CompanyName = "MarketSphere Distribution Bangladesh Ltd.",
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };

            await _db.Companies.AddAsync(
                company,
                cancellationToken);
        }
        else
        {
            company.CompanyName = "MarketSphere Distribution Bangladesh Ltd.";
            company.IsActive = true;
            company.IsDeleted = false;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return company;
    }

    private async Task<Branch> SeedBranchAsync(
        int companyID,
        CancellationToken cancellationToken)
    {
        const string code = "MAIN";

        var branch = await _db.Branches
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.CompanyID == companyID &&
                     x.BranchCode == code,
                cancellationToken);

        if (branch is null)
        {
            branch = new Branch
            {
                CompanyID = companyID,
                BranchCode = code,
                BranchName = "Dhaka Head Office",
                BranchType = BranchType.HeadOffice,
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };

            await _db.Branches.AddAsync(
                branch,
                cancellationToken);
        }
        else
        {
            branch.BranchName = "Dhaka Head Office";
            branch.BranchType = BranchType.HeadOffice;
            branch.IsActive = true;
            branch.IsDeleted = false;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return branch;
    }

    private async Task<Region> SeedRegionAsync(
        int companyID,
        string code,
        string name,
        CancellationToken cancellationToken)
    {
        var region = await _db.Regions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.CompanyID == companyID &&
                     x.RegionCode == code,
                cancellationToken);

        if (region is null)
        {
            region = new Region
            {
                CompanyID = companyID,
                RegionCode = code,
                RegionName = name,
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };

            await _db.Regions.AddAsync(
                region,
                cancellationToken);
        }
        else
        {
            region.RegionName = name;
            region.IsActive = true;
            region.IsDeleted = false;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return region;
    }

    private async Task<Area> SeedAreaAsync(
        int regionID,
        string code,
        string name,
        CancellationToken cancellationToken)
    {
        var area = await _db.Areas
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.RegionID == regionID &&
                     x.AreaCode == code,
                cancellationToken);

        if (area is null)
        {
            area = new Area
            {
                RegionID = regionID,
                AreaCode = code,
                AreaName = name,
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };

            await _db.Areas.AddAsync(
                area,
                cancellationToken);
        }
        else
        {
            area.AreaName = name;
            area.IsActive = true;
            area.IsDeleted = false;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return area;
    }

    private async Task<Territory> SeedTerritoryAsync(
        int areaID,
        string code,
        string name,
        CancellationToken cancellationToken)
    {
        var territory = await _db.Territories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.AreaID == areaID &&
                     x.TerritoryCode == code,
                cancellationToken);

        if (territory is null)
        {
            territory = new Territory
            {
                AreaID = areaID,
                TerritoryCode = code,
                TerritoryName = name,
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };

            await _db.Territories.AddAsync(
                territory,
                cancellationToken);
        }
        else
        {
            territory.TerritoryName = name;
            territory.IsActive = true;
            territory.IsDeleted = false;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return territory;
    }

    private async Task SeedRouteAsync(
        int territoryID,
        string code,
        string name,
        DayOfWeek dayOfWeek,
        CancellationToken cancellationToken)
    {
        var route = await _db.Routes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.TerritoryID == territoryID &&
                     x.RouteCode == code,
                cancellationToken);

        if (route is null)
        {
            route = new Route
            {
                TerritoryID = territoryID,
                RouteCode = code,
                RouteName = name,
                DayOfWeek = dayOfWeek,
                VisitFrequency = VisitFrequency.Weekly,
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };

            await _db.Routes.AddAsync(
                route,
                cancellationToken);
        }
        else
        {
            route.RouteName = name;
            route.DayOfWeek = dayOfWeek;
            route.IsActive = true;
            route.IsDeleted = false;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedAdminEmployeeAsync(
        int branchID,
        CancellationToken cancellationToken)
    {
        var adminEmail = _configuration[
            "BootstrapAdmin:Email"]?
            .Trim()
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(adminEmail))
            return;

        var user = await _db.Users.FirstOrDefaultAsync(
            x => x.Email == adminEmail,
            cancellationToken);

        var designation = await _db.Designations
            .FirstOrDefaultAsync(
                x => x.DesignationCode == "GM",
                cancellationToken);

        if (user is null || designation is null)
            return;

        var employeeExists = await _db.Employees.AnyAsync(
            x => x.UserID == user.UserID,
            cancellationToken);

        if (employeeExists)
            return;

        await _db.Employees.AddAsync(
            new Employee
            {
                EmployeeCode = "EMP-0001",
                UserID = user.UserID,
                DesignationID = designation.DesignationID,
                BranchID = branchID,
                JoiningDate = DateOnly.FromDateTime(
                    _clock.UtcNow),
                Status = EmployeeStatus.Active,
                Email = user.Email,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = user.UserID
            },
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
