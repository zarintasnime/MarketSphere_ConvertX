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

public sealed class PurchaseRequisitionService : IPurchaseRequisitionService
{
    private readonly IApplicationDbContext _db;
    public PurchaseRequisitionService(IApplicationDbContext db) => _db = db;

    public Task<PagedResult<PurchaseRequisitionListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var q = _db.PurchaseRequisitions.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.Trim(); q = q.Where(x => x.PurchaseRequisitionNo.Contains(s) || x.Reason!.Contains(s)); }
        var projected = q.OrderByDescending(x => x.PurchaseRequisitionID).Select(x => new PurchaseRequisitionListDto(
            x.PurchaseRequisitionID, x.PurchaseRequisitionNo, x.Branch.BranchName, x.RequestedByEmployee.EmployeeCode,
            x.RequiredDate, x.Status, x.Items.Sum(i => i.RequestedQuantity * (i.EstimatedUnitCost ?? 0))));
        return InventoryServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<PurchaseRequisitionDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var e = await InventoryServiceHelper.RequireAsync(_db.PurchaseRequisitions.AsNoTracking().Where(x => x.PurchaseRequisitionID == id), "Purchase requisition", cancellationToken);
        var items = await _db.PurchaseRequisitionItems.AsNoTracking().Where(x => x.PurchaseRequisitionID == id).OrderBy(x => x.PurchaseRequisitionItemID)
            .Select(x => new PurchaseRequisitionItemDto(x.PurchaseRequisitionItemID, x.SKUID, x.SKU.SKUCode, x.SKU.SKUName, x.RequestedQuantity, x.EstimatedUnitCost, x.Note)).ToListAsync(cancellationToken);
        return new(e.PurchaseRequisitionID, e.PurchaseRequisitionNo, e.BranchID, e.RequestedByEmployeeID, e.RequiredDate, e.Reason, e.Status, items);
    }

    public async Task<int> CreateAsync(SavePurchaseRequisitionRequestDto request, CancellationToken cancellationToken = default)
    {
        await ValidateRequestAsync(request, null, cancellationToken);
        var e = new PurchaseRequisition(); ApplyHeader(e, request); await _db.AddAsync(e, cancellationToken);
        foreach (var i in request.Items) await _db.AddAsync(new PurchaseRequisitionItem { PurchaseRequisition = e, SKUID = i.SKUID, RequestedQuantity = i.RequestedQuantity, EstimatedUnitCost = i.EstimatedUnitCost, Note = i.Note?.Trim() }, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken); return e.PurchaseRequisitionID;
    }

    public async Task UpdateAsync(int id, SavePurchaseRequisitionRequestDto request, CancellationToken cancellationToken = default)
    {
        var e = await InventoryServiceHelper.RequireAsync(_db.PurchaseRequisitions.Where(x => x.PurchaseRequisitionID == id), "Purchase requisition", cancellationToken);
        if (e.Status != PurchaseRequisitionStatus.Draft) throw new BusinessRuleException("Only a draft purchase requisition can be edited.");
        await ValidateRequestAsync(request, id, cancellationToken); ApplyHeader(e, request);
        var old = await _db.PurchaseRequisitionItems.Where(x => x.PurchaseRequisitionID == id).ToListAsync(cancellationToken); foreach (var x in old) _db.Remove(x);
        foreach (var i in request.Items) await _db.AddAsync(new PurchaseRequisitionItem { PurchaseRequisitionID = id, SKUID = i.SKUID, RequestedQuantity = i.RequestedQuantity, EstimatedUnitCost = i.EstimatedUnitCost, Note = i.Note?.Trim() }, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(int id, ChangePurchaseRequisitionStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var e = await InventoryServiceHelper.RequireAsync(_db.PurchaseRequisitions.Where(x => x.PurchaseRequisitionID == id), "Purchase requisition", cancellationToken);
        var allowed = (e.Status, request.Status) switch
        {
            (PurchaseRequisitionStatus.Draft, PurchaseRequisitionStatus.Submitted) => true,
            (PurchaseRequisitionStatus.Submitted, PurchaseRequisitionStatus.Approved) => true,
            (PurchaseRequisitionStatus.Submitted, PurchaseRequisitionStatus.Rejected) => true,
            (PurchaseRequisitionStatus.Approved, PurchaseRequisitionStatus.Closed) => true,
            (PurchaseRequisitionStatus.Draft, PurchaseRequisitionStatus.Cancelled) => true,
            _ => false
        };
        if (!allowed) throw new BusinessRuleException("The requested status transition is not allowed.");
        e.Status = request.Status; await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateRequestAsync(SavePurchaseRequisitionRequestDto r, int? id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(r.PurchaseRequisitionNo)) throw new BusinessRuleException("Purchase requisition number is required.");
        InventoryServiceHelper.EnsureDistinctPositive(r.Items, x => x.SKUID, x => x.RequestedQuantity);
        if (await _db.PurchaseRequisitions.AnyAsync(x => x.PurchaseRequisitionNo == r.PurchaseRequisitionNo.Trim().ToUpper() && x.PurchaseRequisitionID != id, ct)) throw new ConflictException("Purchase requisition number already exists.");
        if (!await _db.Branches.AnyAsync(x => x.BranchID == r.BranchID && x.IsActive, ct)) throw new BusinessRuleException("An active branch is required.");
        if (!await _db.Employees.AnyAsync(x => x.EmployeeID == r.RequestedByEmployeeID, ct)) throw new BusinessRuleException("Requested employee was not found.");
        var ids = r.Items.Select(x => x.SKUID).ToArray(); if (await _db.SKUs.CountAsync(x => ids.Contains(x.SKUID) && x.IsActive, ct) != ids.Length) throw new BusinessRuleException("Every requisition item must use an active SKU.");
        if (r.Items.Any(x => x.EstimatedUnitCost < 0)) throw new BusinessRuleException("Estimated unit cost cannot be negative.");
    }
    private static void ApplyHeader(PurchaseRequisition e, SavePurchaseRequisitionRequestDto r) { e.PurchaseRequisitionNo = r.PurchaseRequisitionNo.Trim().ToUpperInvariant(); e.BranchID = r.BranchID; e.RequestedByEmployeeID = r.RequestedByEmployeeID; e.RequiredDate = r.RequiredDate.Date; e.Reason = r.Reason?.Trim(); }
}
