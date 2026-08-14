namespace MarketSphere.Application.Common.Interfaces;

public interface IKpiProjectionService
{
    Task<decimal> GetActualValueAsync(
        int employeeID,
        int targetType,
        DateTime periodStart,
        DateTime periodEnd,
        int? campaignID,
        int? skuID,
        int? clientID,
        CancellationToken cancellationToken = default);
}
