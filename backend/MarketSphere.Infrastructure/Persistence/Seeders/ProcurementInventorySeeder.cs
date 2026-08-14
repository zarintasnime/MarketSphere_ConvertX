using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.Procurement;
using MarketSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Infrastructure.Persistence.Seeders;

public sealed class ProcurementInventorySeeder
{
    private readonly MarketSphereDbContext _db;
    private readonly IDateTimeProvider _clock;

    public ProcurementInventorySeeder(MarketSphereDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var branchID = await _db.Branches
            .OrderBy(x => x.BranchID)
            .Select(x => x.BranchID)
            .FirstAsync(cancellationToken);

        await EnsureWarehouseAsync(branchID, "MAIN-WH", "Tejgaon Central Warehouse", "Tejgaon Industrial Area, Dhaka", WarehouseType.Main, cancellationToken);
        await EnsureWarehouseAsync(branchID, "RET-WH", "Dhaka Returns & Quarantine Warehouse", "Jatrabari, Dhaka", WarehouseType.Returns, cancellationToken);

        var suppliers = new[]
        {
            new SupplierSeed("BD-SUP-001", "Meghna Distribution Services", "Tanvir Hasan", "01711000001", "supply@meghna-demo.local", "Tejgaon I/A, Dhaka", 30),
            new SupplierSeed("BD-SUP-002", "Karnaphuli FMCG Supply", "Nusrat Jahan", "01812000002", "sales@karnaphuli-demo.local", "Agrabad C/A, Chattogram", 21),
            new SupplierSeed("BD-SUP-003", "Padma Wholesale Traders", "Arif Hossain", "01913000003", "orders@padma-demo.local", "Jatrabari, Dhaka", 14),
        };

        var supplierEntities = new List<Supplier>();
        foreach (var seed in suppliers)
        {
            supplierEntities.Add(await EnsureSupplierAsync(seed, cancellationToken));
        }

        var skus = await _db.SKUs
            .Where(x => x.IsActive)
            .OrderBy(x => x.SKUID)
            .ToListAsync(cancellationToken);

        for (var supplierIndex = 0; supplierIndex < supplierEntities.Count; supplierIndex++)
        {
            var supplier = supplierEntities[supplierIndex];
            foreach (var sku in skus.Where((_, skuIndex) => supplierIndex == 0 || skuIndex % supplierEntities.Count == supplierIndex))
            {
                if (await _db.SupplierProducts.AnyAsync(
                        x => x.SupplierID == supplier.SupplierID && x.SKUID == sku.SKUID,
                        cancellationToken))
                {
                    continue;
                }

                await _db.SupplierProducts.AddAsync(new SupplierProduct
                {
                    SupplierID = supplier.SupplierID,
                    SKUID = sku.SKUID,
                    MinimumOrderQuantity = supplierIndex == 0 ? 24m : 12m,
                    LeadTimeDays = 3 + supplierIndex * 2,
                    IsPreferredSupplier = supplierIndex == 0,
                    IsActive = true,
                    CreatedAt = _clock.UtcNow
                }, cancellationToken);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Supplier> EnsureSupplierAsync(SupplierSeed seed, CancellationToken cancellationToken)
    {
        var supplier = await _db.Suppliers
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.SupplierCode == seed.Code, cancellationToken);

        if (supplier is null)
        {
            supplier = new Supplier
            {
                SupplierCode = seed.Code,
                SupplierName = seed.Name,
                ContactPerson = seed.ContactPerson,
                Phone = seed.Phone,
                Email = seed.Email,
                Address = seed.Address,
                PaymentTermsDays = seed.PaymentTermsDays,
                Status = SupplierStatus.Active,
                CreatedAt = _clock.UtcNow
            };
            await _db.Suppliers.AddAsync(supplier, cancellationToken);
        }
        else
        {
            supplier.SupplierName = seed.Name;
            supplier.ContactPerson = seed.ContactPerson;
            supplier.Phone = seed.Phone;
            supplier.Email = seed.Email;
            supplier.Address = seed.Address;
            supplier.PaymentTermsDays = seed.PaymentTermsDays;
            supplier.Status = SupplierStatus.Active;
            supplier.IsDeleted = false;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return supplier;
    }

    private async Task EnsureWarehouseAsync(
        int branchID,
        string code,
        string name,
        string address,
        WarehouseType type,
        CancellationToken cancellationToken)
    {
        var warehouse = await _db.Warehouses
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(
                x => x.BranchID == branchID && x.WarehouseCode == code,
                cancellationToken);

        if (warehouse is null)
        {
            await _db.Warehouses.AddAsync(new Warehouse
            {
                BranchID = branchID,
                WarehouseCode = code,
                WarehouseName = name,
                Address = address,
                WarehouseType = type,
                IsActive = true,
                CreatedAt = _clock.UtcNow
            }, cancellationToken);
        }
        else
        {
            warehouse.WarehouseName = name;
            warehouse.Address = address;
            warehouse.WarehouseType = type;
            warehouse.IsActive = true;
            warehouse.IsDeleted = false;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private sealed record SupplierSeed(
        string Code,
        string Name,
        string ContactPerson,
        string Phone,
        string Email,
        string Address,
        int PaymentTermsDays);
}
