using MarketSphere.Application.Modules.Infrastructure.DTOs;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.Infrastructure.Services;

public sealed class LookupService : ILookupService
{
    private static readonly IReadOnlyDictionary<string, Type> EnumMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
    {
        ["sales-channels"] = typeof(SalesChannel),
        ["user-statuses"] = typeof(UserStatus),
        ["lead-statuses"] = typeof(LeadStatus),
        ["opportunity-stages"] = typeof(OpportunityStage),
        ["quotation-statuses"] = typeof(QuotationStatus),
        ["campaign-statuses"] = typeof(CampaignStatus),
        ["order-statuses"] = typeof(OrderStatus),
        ["delivery-statuses"] = typeof(DeliveryStatus),
        ["return-statuses"] = typeof(ReturnRequestStatus),
        ["payment-methods"] = typeof(PaymentMethod),
        ["target-types"] = typeof(TargetType),
        ["reward-statuses"] = typeof(RewardCalculationStatus),
        ["approval-types"] = typeof(ApprovalType),
        ["approval-statuses"] = typeof(ApprovalRequestStatus),
        ["notification-priorities"] = typeof(NotificationPriority),
        ["offline-sync-statuses"] = typeof(OfflineSyncStatus)
    };

    public Task<LookupGroupDto> GetAsync(string code, CancellationToken cancellationToken = default)
    {
        if (!EnumMap.TryGetValue(code.Trim(), out var enumType)) throw new NotFoundException("Lookup group was not found.");
        var items = Enum.GetValues(enumType).Cast<object>().Select(x => new LookupItemDto(Convert.ToInt32(x), SplitName(x.ToString()!), null, true)).ToArray();
        return Task.FromResult(new LookupGroupDto(code.Trim().ToLowerInvariant(), items));
    }

    private static string SplitName(string value) => System.Text.RegularExpressions.Regex.Replace(value, "([a-z0-9])([A-Z])", "$1 $2");
}
