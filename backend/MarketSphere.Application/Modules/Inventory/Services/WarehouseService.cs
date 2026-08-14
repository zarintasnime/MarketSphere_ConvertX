using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Modules.Inventory.DTOs;
using MarketSphere.Application.Modules.Inventory.Interfaces;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.Inventory.Services;

public sealed class WarehouseService : IWarehouseService
{
    private readonly IApplicationDbContext _db;
    public WarehouseService(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<WarehouseDto>> GetAsync(CancellationToken cancellationToken = default)
        => await _db.Warehouses.AsNoTracking().OrderBy(x => x.WarehouseName)
            .Select(x => new WarehouseDto(x.WarehouseID, x.BranchID, x.Branch.BranchName,
                x.WarehouseCode, x.WarehouseName, x.WarehouseType, x.Address, x.IsActive))
            .ToListAsync(cancellationToken);

    public async Task<int> CreateAsync(SaveWarehouseRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request); var code = request.WarehouseCode.Trim().ToUpperInvariant();
        if (!await _db.Branches.AnyAsync(x => x.BranchID == request.BranchID && x.IsActive, cancellationToken)) throw new BusinessRuleException("An active branch is required.");
        if (await _db.Warehouses.AnyAsync(x => x.BranchID == request.BranchID && x.WarehouseCode == code, cancellationToken)) throw new ConflictException("Warehouse code already exists in the branch.");
        var e = new Warehouse(); Apply(e, request, code); await _db.AddAsync(e, cancellationToken); await _db.SaveChangesAsync(cancellationToken); return e.WarehouseID;
    }

    public async Task UpdateAsync(int id, SaveWarehouseRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request); var e = await InventoryServiceHelper.RequireAsync(_db.Warehouses.Where(x => x.WarehouseID == id), "Warehouse", cancellationToken); var code = request.WarehouseCode.Trim().ToUpperInvariant();
        if (!await _db.Branches.AnyAsync(x => x.BranchID == request.BranchID && x.IsActive, cancellationToken)) throw new BusinessRuleException("An active branch is required.");
        if (await _db.Warehouses.AnyAsync(x => x.BranchID == request.BranchID && x.WarehouseCode == code && x.WarehouseID != id, cancellationToken)) throw new ConflictException("Warehouse code already exists in the branch.");
        Apply(e, request, code); await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(int id, ChangeWarehouseStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var e = await InventoryServiceHelper.RequireAsync(_db.Warehouses.Where(x => x.WarehouseID == id), "Warehouse", cancellationToken);
        if (!request.IsActive && await _db.StockBalances.AnyAsync(x => x.WarehouseID == id && x.OnHandQuantity > 0, cancellationToken)) throw new BusinessRuleException("A warehouse with stock cannot be deactivated.");
        e.IsActive = request.IsActive; await _db.SaveChangesAsync(cancellationToken);
    }

    private static void Validate(SaveWarehouseRequestDto r) { if (string.IsNullOrWhiteSpace(r.WarehouseCode) || string.IsNullOrWhiteSpace(r.WarehouseName)) throw new BusinessRuleException("Warehouse code and name are required."); }
    private static void Apply(Warehouse e, SaveWarehouseRequestDto r, string code) { e.BranchID = r.BranchID; e.WarehouseCode = code; e.WarehouseName = r.WarehouseName.Trim(); e.WarehouseType = r.WarehouseType; e.Address = r.Address?.Trim(); }
}
