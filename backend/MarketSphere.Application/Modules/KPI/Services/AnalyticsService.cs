using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Modules.KPI.DTOs;
using MarketSphere.Application.Modules.KPI.Interfaces;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.KPI.Services;

public sealed class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsQueryService _queries;

    public AnalyticsService(IAnalyticsQueryService queries)
    {
        _queries = queries;
    }

    public Task<ExecutiveDashboardDto> GetExecutiveDashboardAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        Validate(filter);
        return _queries.GetExecutiveDashboardAsync(filter, cancellationToken);
    }

    public Task<IReadOnlyCollection<FunnelPointDto>> GetLeadToOrderFunnelAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        Validate(filter);
        return _queries.GetLeadToOrderFunnelAsync(filter, cancellationToken);
    }

    public Task<IReadOnlyCollection<SeriesPointDto>> GetSalesTrendAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        Validate(filter);
        return _queries.GetSalesTrendAsync(filter, cancellationToken);
    }

    public Task<IReadOnlyCollection<CampaignRoiDto>> GetCampaignRoiAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        Validate(filter);
        return _queries.GetCampaignRoiAsync(filter, cancellationToken);
    }

    public Task<IReadOnlyCollection<ChannelSalesDto>> GetChannelSalesAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        Validate(filter);
        return _queries.GetChannelSalesAsync(filter, cancellationToken);
    }

    public Task<IReadOnlyCollection<SellInSellOutPointDto>> GetSellInSellOutAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        Validate(filter);
        return _queries.GetSellInSellOutAsync(filter, cancellationToken);
    }

    public Task<InventoryHealthDto> GetInventoryHealthAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        Validate(filter);
        return _queries.GetInventoryHealthAsync(filter, cancellationToken);
    }

    public Task<IReadOnlyCollection<DeliveryReturnPointDto>> GetDeliveryReturnAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        Validate(filter);
        return _queries.GetDeliveryReturnAsync(filter, cancellationToken);
    }

    public Task<IReadOnlyCollection<EmployeeKpiDto>> GetEmployeeKpiAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        Validate(filter);
        return _queries.GetEmployeeKpiAsync(filter, cancellationToken);
    }

    public Task<Client360Dto> GetClient360Async(
        int clientID,
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        if (clientID <= 0)
        {
            throw new BusinessRuleException(
                "ClientID must be greater than zero.");
        }

        Validate(filter);
        return _queries.GetClient360Async(clientID, filter, cancellationToken);
    }

    private static void Validate(AnalyticsFilterDto filter)
    {
        if (filter.To < filter.From)
        {
            throw new BusinessRuleException(
                "Analytics end date cannot be earlier than start date.");
        }

        if ((filter.To - filter.From).TotalDays > 3660)
        {
            throw new BusinessRuleException(
                "Analytics date range is too large.");
        }

        ValidateOptionalID(filter.BranchID, nameof(filter.BranchID));
        ValidateOptionalID(filter.RegionID, nameof(filter.RegionID));
        ValidateOptionalID(filter.EmployeeID, nameof(filter.EmployeeID));
        ValidateOptionalID(filter.CampaignID, nameof(filter.CampaignID));
    }

    private static void ValidateOptionalID(int? value, string name)
    {
        if (value.HasValue && value.Value <= 0)
        {
            throw new BusinessRuleException(
                $"{name} must be greater than zero when supplied.");
        }
    }
}
