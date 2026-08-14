using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Enums;
using MarketSphere.Infrastructure.Persistence;

namespace MarketSphere.Infrastructure.Reports;

public sealed class KpiProjectionService : IKpiProjectionService
{
    private readonly MarketSphereDbContext _db;
    public KpiProjectionService(MarketSphereDbContext db) => _db = db;

    public async Task<decimal> GetActualValueAsync(int employeeID, int targetType, DateTime periodStart, DateTime periodEnd, int? campaignID, int? skuID, int? clientID, CancellationToken cancellationToken = default)
    {
        var type = (TargetType)targetType;
        var startDate = DateOnly.FromDateTime(periodStart);
        var endDate = DateOnly.FromDateTime(periodEnd);
        return type switch
        {
            TargetType.SalesAmount => await _db.Orders.AsNoTracking().Where(x => x.EmployeeID == employeeID && x.OrderDate >= periodStart && x.OrderDate <= periodEnd && (!campaignID.HasValue || x.CampaignID == campaignID) && (!clientID.HasValue || x.ClientID == clientID) && x.Status != OrderStatus.Rejected && x.Status != OrderStatus.Cancelled).SumAsync(x => (decimal?)x.NetAmount, cancellationToken) ?? 0,
            TargetType.SalesQuantity => await _db.DeliveryItems.AsNoTracking().Where(x => x.Delivery.DeliveredByEmployeeID == employeeID && x.Delivery.DeliveredAt >= periodStart && x.Delivery.DeliveredAt <= periodEnd && (!skuID.HasValue || x.SKUID == skuID) && (!clientID.HasValue || x.Delivery.Order.ClientID == clientID)).SumAsync(x => (decimal?)x.QuantityDelivered, cancellationToken) ?? 0,
            TargetType.CollectionAmount => await _db.Payments.AsNoTracking().Where(x => x.ReceivedByUser.Employee!.EmployeeID == employeeID && x.PaymentDate >= periodStart && x.PaymentDate <= periodEnd && (!clientID.HasValue || x.ClientID == clientID) && x.Status == CustomerPaymentStatus.Confirmed).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0,
            TargetType.VisitCount => await _db.Visits.AsNoTracking().CountAsync(x => x.EmployeeID == employeeID && x.CheckInAt >= periodStart && x.CheckInAt <= periodEnd && (!campaignID.HasValue || x.CampaignID == campaignID) && (!clientID.HasValue || x.ClientID == clientID), cancellationToken),
            TargetType.NewClientCount => await _db.Clients.AsNoTracking().CountAsync(x => x.CreatedByUserID == _db.Employees.Where(e => e.EmployeeID == employeeID).Select(e => e.UserID).FirstOrDefault() && x.CreatedAt >= periodStart && x.CreatedAt <= periodEnd && (!clientID.HasValue || x.ClientID == clientID), cancellationToken),
            TargetType.VerifiedSellOutAmount => await _db.BPSellOuts.AsNoTracking().Where(x => x.EmployeeID == employeeID && x.SellOutDate >= startDate && x.SellOutDate <= endDate && (!campaignID.HasValue || x.CampaignID == campaignID) && (!clientID.HasValue || x.ClientID == clientID) && x.VerificationStatus == VerificationStatus.Verified).SumAsync(x => (decimal?)x.TotalValue, cancellationToken) ?? 0,
            TargetType.CampaignAchievement => campaignID.HasValue ? await _db.CampaignAttributions.AsNoTracking().Where(x => x.CampaignID == campaignID && x.AttributedAmount.HasValue).SumAsync(x => x.AttributedAmount, cancellationToken) ?? 0 : 0,
            _ => 0
        };
    }
}
