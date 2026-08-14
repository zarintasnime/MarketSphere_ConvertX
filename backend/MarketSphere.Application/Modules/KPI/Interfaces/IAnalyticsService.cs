using MarketSphere.Application.Modules.KPI.DTOs;

namespace MarketSphere.Application.Modules.KPI.Interfaces;

public interface IAnalyticsService
{
    Task<ExecutiveDashboardDto> GetExecutiveDashboardAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FunnelPointDto>> GetLeadToOrderFunnelAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SeriesPointDto>> GetSalesTrendAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CampaignRoiDto>> GetCampaignRoiAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ChannelSalesDto>> GetChannelSalesAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SellInSellOutPointDto>> GetSellInSellOutAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<InventoryHealthDto> GetInventoryHealthAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DeliveryReturnPointDto>> GetDeliveryReturnAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EmployeeKpiDto>> GetEmployeeKpiAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Client360Dto> GetClient360Async(
        int clientID,
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default);
}
