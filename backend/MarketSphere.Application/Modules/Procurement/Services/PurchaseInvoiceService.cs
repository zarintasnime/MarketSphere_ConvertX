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

public sealed class PurchaseInvoiceService : IPurchaseInvoiceService
{
    private readonly IApplicationDbContext _db;
    public PurchaseInvoiceService(IApplicationDbContext db) => _db = db;

    public Task<PagedResult<PurchaseInvoiceDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var q = _db.PurchaseInvoices.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.Trim(); q = q.Where(x => x.PurchaseInvoiceNo.Contains(s) || x.Supplier.SupplierName.Contains(s)); }
        var p = q.OrderByDescending(x => x.PurchaseInvoiceID).Select(x => new PurchaseInvoiceDto(x.PurchaseInvoiceID, x.PurchaseInvoiceNo, x.SupplierID, x.Supplier.SupplierName, x.PurchaseOrderID, x.GoodsReceiptID, x.InvoiceDate, x.DueDate, x.TotalAmount, x.PaidAmount, x.DueAmount, x.PaymentStatus, x.Status));
        return InventoryServiceHelper.ToPagedAsync(p, request, cancellationToken);
    }

    public async Task<int> CreateAsync(SavePurchaseInvoiceRequestDto r, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(r.PurchaseInvoiceNo)) throw new BusinessRuleException("Purchase invoice number is required.");
        if (r.GrossAmount < 0 || r.DiscountAmount < 0 || r.TaxAmount < 0) throw new BusinessRuleException("Invoice amounts cannot be negative.");
        var total = r.GrossAmount - r.DiscountAmount + r.TaxAmount; if (total < 0) throw new BusinessRuleException("Invoice total cannot be negative.");
        if (await _db.PurchaseInvoices.AnyAsync(x => x.SupplierID == r.SupplierID && x.PurchaseInvoiceNo == r.PurchaseInvoiceNo.Trim().ToUpper(), ct)) throw new ConflictException("Supplier invoice number already exists for this supplier.");
        if (!await _db.Suppliers.AnyAsync(x => x.SupplierID == r.SupplierID && x.Status == SupplierStatus.Active, ct)) throw new BusinessRuleException("An active supplier is required.");
        if (r.PurchaseOrderID.HasValue && !await _db.PurchaseOrders.AnyAsync(x => x.PurchaseOrderID == r.PurchaseOrderID && x.SupplierID == r.SupplierID, ct)) throw new BusinessRuleException("Purchase order does not belong to the supplier.");
        if (r.GoodsReceiptID.HasValue && !await _db.GoodsReceipts.AnyAsync(x => x.GoodsReceiptID == r.GoodsReceiptID && x.Status == GoodsReceiptStatus.Posted, ct)) throw new BusinessRuleException("A posted goods receipt is required.");
        var e = new PurchaseInvoice { PurchaseInvoiceNo = r.PurchaseInvoiceNo.Trim().ToUpperInvariant(), SupplierID = r.SupplierID, PurchaseOrderID = r.PurchaseOrderID, GoodsReceiptID = r.GoodsReceiptID, InvoiceDate = r.InvoiceDate.Date, DueDate = r.DueDate?.Date, GrossAmount = r.GrossAmount, DiscountAmount = r.DiscountAmount, TaxAmount = r.TaxAmount, TotalAmount = total, DueAmount = total };
        await _db.AddAsync(e, ct); await _db.SaveChangesAsync(ct); return e.PurchaseInvoiceID;
    }

    public async Task ConfirmAsync(int id, CancellationToken ct = default)
    { var e = await InventoryServiceHelper.RequireAsync(_db.PurchaseInvoices.Where(x => x.PurchaseInvoiceID == id), "Purchase invoice", ct); if (e.Status != PurchaseInvoiceStatus.Draft) throw new BusinessRuleException("Only a draft purchase invoice can be confirmed."); e.Status = PurchaseInvoiceStatus.Confirmed; await _db.SaveChangesAsync(ct); }

    public async Task<int> CreatePaymentAsync(CreateSupplierPaymentRequestDto r, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(r.PaymentNo) || r.Amount <= 0) throw new BusinessRuleException("Payment number and positive amount are required.");
        var invoice = await InventoryServiceHelper.RequireAsync(_db.PurchaseInvoices.Where(x => x.PurchaseInvoiceID == r.PurchaseInvoiceID), "Purchase invoice", ct);
        if (invoice.Status != PurchaseInvoiceStatus.Confirmed) throw new BusinessRuleException("A confirmed purchase invoice is required.");
        if (r.Amount > invoice.DueAmount) throw new BusinessRuleException("Payment amount cannot exceed invoice due amount.");
        if (await _db.SupplierPayments.AnyAsync(x => x.PaymentNo == r.PaymentNo.Trim().ToUpper(), ct)) throw new ConflictException("Supplier payment number already exists.");
        var p = new SupplierPayment { SupplierID = invoice.SupplierID, PurchaseInvoiceID = invoice.PurchaseInvoiceID, PaymentNo = r.PaymentNo.Trim().ToUpperInvariant(), PaymentDate = r.PaymentDate.Date, PaymentMethod = r.PaymentMethod, Amount = r.Amount, ReferenceNo = r.ReferenceNo?.Trim() }; await _db.AddAsync(p, ct); await _db.SaveChangesAsync(ct); return p.SupplierPaymentID;
    }

    public async Task ChangePaymentStatusAsync(int supplierPaymentID, ChangeSupplierPaymentStatusRequestDto r, CancellationToken ct = default)
    {
        await _db.ExecuteInTransactionAsync(async token =>
        {
            var p = await _db.SupplierPayments.SingleOrDefaultAsync(x => x.SupplierPaymentID == supplierPaymentID, token) ?? throw new NotFoundException("Supplier payment was not found.");
            var invoice = await InventoryServiceHelper.RequireAsync(_db.PurchaseInvoices.Where(x => x.PurchaseInvoiceID == p.PurchaseInvoiceID), "Purchase invoice", token);
            if (p.Status == SupplierPaymentStatus.Pending && r.Status == SupplierPaymentStatus.Confirmed) { if (p.Amount > invoice.DueAmount) throw new BusinessRuleException("Payment amount exceeds current invoice due."); invoice.PaidAmount += p.Amount; invoice.DueAmount -= p.Amount; p.Status = r.Status; }
            else if (p.Status == SupplierPaymentStatus.Pending && (r.Status == SupplierPaymentStatus.Rejected)) { p.Status = r.Status; }
            else if (p.Status == SupplierPaymentStatus.Confirmed && r.Status == SupplierPaymentStatus.Reversed) { invoice.PaidAmount -= p.Amount; invoice.DueAmount += p.Amount; p.Status = r.Status; }
            else throw new BusinessRuleException("The requested supplier-payment transition is not allowed.");
            invoice.PaymentStatus = invoice.DueAmount == 0 ? SupplierInvoicePaymentStatus.Paid : invoice.PaidAmount > 0 ? SupplierInvoicePaymentStatus.PartiallyPaid : SupplierInvoicePaymentStatus.Unpaid;
            await _db.SaveChangesAsync(token); return true;
        }, ct);
    }

    public async Task<IReadOnlyCollection<SupplierPaymentDto>> GetPaymentsAsync(int purchaseInvoiceID, CancellationToken ct = default)
        => await _db.SupplierPayments.AsNoTracking().Where(x => x.PurchaseInvoiceID == purchaseInvoiceID).OrderByDescending(x => x.SupplierPaymentID)
            .Select(x => new SupplierPaymentDto(x.SupplierPaymentID, x.PaymentNo, x.PurchaseInvoiceID, x.PaymentDate, x.PaymentMethod, x.Amount, x.ReferenceNo, x.Status)).ToListAsync(ct);
}
