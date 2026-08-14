using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.Infrastructure;

namespace MarketSphere.Infrastructure.Persistence.Seeders;

public sealed class NumberSequenceSeeder
{
    private readonly MarketSphereDbContext _db; private readonly IDateTimeProvider _clock;
    public NumberSequenceSeeder(MarketSphereDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var year = _clock.UtcNow.Year;
        var values = new Dictionary<string, string> { { DocumentTypeCodes.PurchaseRequisition, "PR" }, { DocumentTypeCodes.PurchaseOrder, "PO" }, { DocumentTypeCodes.GoodsReceipt, "GRN" }, { DocumentTypeCodes.PurchaseInvoice, "PINV" }, { DocumentTypeCodes.SupplierPayment, "SPAY" }, { DocumentTypeCodes.SupplierReturn, "SRET" }, { DocumentTypeCodes.StockTransfer, "STR" }, { DocumentTypeCodes.StockAdjustment, "SADJ" }, { DocumentTypeCodes.Order, "ORD" }, { DocumentTypeCodes.Invoice, "INV" }, { DocumentTypeCodes.PickList, "PICK" }, { DocumentTypeCodes.Delivery, "DEL" }, { DocumentTypeCodes.Return, "RET" }, { DocumentTypeCodes.CreditNote, "CN" }, { DocumentTypeCodes.Payment, "PAY" } };
        foreach (var pair in values) if (!await _db.NumberSequences.AnyAsync(x => x.DocumentType == pair.Key && x.YearValue == year && x.BranchID == null, cancellationToken)) await _db.NumberSequences.AddAsync(new NumberSequence { DocumentType = pair.Key, Prefix = pair.Value, YearValue = year, LastNumber = 0, PaddingLength = 6, ResetPolicy = "YEARLY", CreatedAt = _clock.UtcNow }, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
