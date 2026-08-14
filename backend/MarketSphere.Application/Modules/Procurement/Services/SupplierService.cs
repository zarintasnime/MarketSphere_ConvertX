using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Inventory.Services;
using MarketSphere.Application.Modules.Procurement.DTOs;
using MarketSphere.Application.Modules.Procurement.Interfaces;
using MarketSphere.Domain.Entities.Procurement;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.Procurement.Services;

public sealed class SupplierService : ISupplierService
{
    private readonly IApplicationDbContext _db;
    public SupplierService(IApplicationDbContext db) => _db = db;

    public Task<PagedResult<SupplierListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.Suppliers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.SupplierCode.Contains(search) || x.SupplierName.Contains(search));
        }
        return InventoryServiceHelper.ToPagedAsync(query.OrderBy(x => x.SupplierName).Select(x =>
            new SupplierListDto(x.SupplierID, x.SupplierCode, x.SupplierName, x.Phone, x.PaymentTermsDays, x.Status)), request, cancellationToken);
    }

    public async Task<SupplierDetailsDto> GetByIdAsync(int supplierID, CancellationToken cancellationToken = default)
    {
        var supplier = await InventoryServiceHelper.RequireAsync(
            _db.Suppliers.AsNoTracking().Where(x => x.SupplierID == supplierID), "Supplier", cancellationToken);
        var products = await _db.SupplierProducts.AsNoTracking()
            .Where(x => x.SupplierID == supplierID)
            .OrderBy(x => x.SKU.SKUName)
            .Select(x => new SupplierProductDto(x.SupplierProductID, x.SKUID, x.SKU.SKUCode, x.SKU.SKUName,
                x.SupplierSKUCode, x.LastPurchasePrice, x.MinimumOrderQuantity, x.LeadTimeDays, x.IsPreferredSupplier, x.IsActive))
            .ToListAsync(cancellationToken);
        return new SupplierDetailsDto(supplier.SupplierID, supplier.SupplierCode, supplier.SupplierName,
            supplier.ContactPerson, supplier.Phone, supplier.Email, supplier.Address, supplier.PaymentTermsDays, supplier.Status, products);
    }

    public async Task<int> CreateAsync(SaveSupplierRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        var code = request.SupplierCode.Trim().ToUpperInvariant();
        if (await _db.Suppliers.AnyAsync(x => x.SupplierCode == code, cancellationToken))
            throw new ConflictException("Supplier code already exists.");
        var entity = new Supplier(); Apply(entity, request, code);
        await _db.AddAsync(entity, cancellationToken); await _db.SaveChangesAsync(cancellationToken); return entity.SupplierID;
    }

    public async Task UpdateAsync(int supplierID, SaveSupplierRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        var entity = await InventoryServiceHelper.RequireAsync(_db.Suppliers.Where(x => x.SupplierID == supplierID), "Supplier", cancellationToken);
        var code = request.SupplierCode.Trim().ToUpperInvariant();
        if (await _db.Suppliers.AnyAsync(x => x.SupplierCode == code && x.SupplierID != supplierID, cancellationToken))
            throw new ConflictException("Supplier code already exists.");
        Apply(entity, request, code); await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(int supplierID, ChangeSupplierStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await InventoryServiceHelper.RequireAsync(_db.Suppliers.Where(x => x.SupplierID == supplierID), "Supplier", cancellationToken);
        if (request.Status == SupplierStatus.Inactive && await _db.PurchaseOrders.AnyAsync(
            x => x.SupplierID == supplierID && x.Status != PurchaseOrderStatus.Closed && x.Status != PurchaseOrderStatus.Cancelled,
            cancellationToken))
            throw new BusinessRuleException("A supplier with open purchase orders cannot be deactivated.");
        entity.Status = request.Status; await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> UpsertProductAsync(int supplierID, SaveSupplierProductRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!await _db.Suppliers.AnyAsync(x => x.SupplierID == supplierID && x.Status == SupplierStatus.Active, cancellationToken))
            throw new BusinessRuleException("An active supplier is required.");
        if (!await _db.SKUs.AnyAsync(x => x.SKUID == request.SKUID && x.IsActive, cancellationToken))
            throw new BusinessRuleException("An active SKU is required.");
        if (request.LastPurchasePrice < 0 || request.MinimumOrderQuantity <= 0 || request.LeadTimeDays < 0)
            throw new BusinessRuleException("Supplier-product values are invalid.");

        var entity = await _db.SupplierProducts.SingleOrDefaultAsync(
            x => x.SupplierID == supplierID && x.SKUID == request.SKUID, cancellationToken);
        if (entity is null)
        {
            entity = new SupplierProduct { SupplierID = supplierID, SKUID = request.SKUID };
            await _db.AddAsync(entity, cancellationToken);
        }
        entity.SupplierSKUCode = string.IsNullOrWhiteSpace(request.SupplierSKUCode) ? null : request.SupplierSKUCode.Trim();
        entity.LastPurchasePrice = request.LastPurchasePrice;
        entity.MinimumOrderQuantity = request.MinimumOrderQuantity;
        entity.LeadTimeDays = request.LeadTimeDays;
        entity.IsPreferredSupplier = request.IsPreferredSupplier;
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken); return entity.SupplierProductID;
    }

    private static void Validate(SaveSupplierRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.SupplierCode) || string.IsNullOrWhiteSpace(request.SupplierName))
            throw new BusinessRuleException("Supplier code and name are required.");
        if (request.PaymentTermsDays < 0) throw new BusinessRuleException("Payment terms cannot be negative.");
    }
    private static void Apply(Supplier e, SaveSupplierRequestDto r, string code)
    {
        e.SupplierCode = code; e.SupplierName = r.SupplierName.Trim(); e.ContactPerson = r.ContactPerson?.Trim();
        e.Phone = r.Phone?.Trim(); e.Email = r.Email?.Trim(); e.Address = r.Address?.Trim(); e.PaymentTermsDays = r.PaymentTermsDays;
    }
}
