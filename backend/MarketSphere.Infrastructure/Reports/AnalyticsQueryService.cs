using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Modules.KPI.DTOs;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using MarketSphere.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Infrastructure.Reports;

public sealed class AnalyticsQueryService : IAnalyticsQueryService
{
    private const decimal LowStockThreshold = 10m;
    private readonly MarketSphereDbContext _db;
    private readonly IDateTimeProvider _clock;

    public AnalyticsQueryService(MarketSphereDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<ExecutiveDashboardDto> GetExecutiveDashboardAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var validOrders = ApplyOrderFilters(filter)
            .Where(x =>
                x.Status != OrderStatus.Rejected &&
                x.Status != OrderStatus.Cancelled);

        var orderIDs = validOrders.Select(x => x.OrderID);
        var clientIDs = validOrders.Select(x => x.ClientID).Distinct();

        var sales = await validOrders.SumAsync(
            x => (decimal?)x.NetAmount,
            cancellationToken) ?? 0m;

        var payments = _db.Payments
            .AsNoTracking()
            .Where(x =>
                x.PaymentDate >= filter.From &&
                x.PaymentDate <= filter.To &&
                x.Status == CustomerPaymentStatus.Confirmed);

        if (filter.RegionID.HasValue)
        {
            payments = payments.Where(
                x => x.Client.RegionID == filter.RegionID.Value);
        }

        if (HasEmployeeDrivenFilter(filter))
        {
            payments = payments.Where(x => clientIDs.Contains(x.ClientID));
        }

        var collections = await payments.SumAsync(
            x => (decimal?)x.Amount,
            cancellationToken) ?? 0m;

        var delivered = await _db.Deliveries
            .AsNoTracking()
            .CountAsync(
                x => orderIDs.Contains(x.OrderID) &&
                     x.DeliveredAt.HasValue &&
                     x.DeliveredAt.Value >= filter.From &&
                     x.DeliveredAt.Value <= filter.To &&
                     x.Status == DeliveryStatus.Delivered,
                cancellationToken);

        var clients = _db.Clients
            .AsNoTracking()
            .Where(x => x.IsActive);

        if (filter.RegionID.HasValue)
        {
            clients = clients.Where(
                x => x.RegionID == filter.RegionID.Value);
        }

        if (HasEmployeeDrivenFilter(filter))
        {
            clients = clients.Where(x => clientIDs.Contains(x.ClientID));
        }

        var activeClients = await clients.CountAsync(cancellationToken);

        var pendingApprovals = await _db.ApprovalRequests
            .AsNoTracking()
            .CountAsync(
                x => x.Status == ApprovalRequestStatus.Pending ||
                     x.Status == ApprovalRequestStatus.InProgress,
                cancellationToken);

        var now = _clock.UtcNow;
        var nearExpiryCutoff = now.AddDays(30);

        var overdueTasksQuery = _db.CRMTasks
            .AsNoTracking()
            .Where(x =>
                x.DueAt < now &&
                x.Status != CrmTaskStatus.Completed &&
                x.Status != CrmTaskStatus.Cancelled);

        if (filter.EmployeeID.HasValue)
        {
            overdueTasksQuery = overdueTasksQuery.Where(
                x => x.AssignedEmployeeID == filter.EmployeeID.Value);
        }

        var overdueTasks = await overdueTasksQuery.CountAsync(
            cancellationToken);

        var nearExpiryBatchesQuery = _db.StockBalances
            .AsNoTracking()
            .Where(x =>
                x.BatchID.HasValue &&
                x.Batch!.ExpiryDate.HasValue &&
                x.Batch.ExpiryDate.Value >= now &&
                x.Batch.ExpiryDate.Value <= nearExpiryCutoff &&
                x.Batch.Status == BatchStatus.Available &&
                x.OnHandQuantity > 0);

        if (filter.BranchID.HasValue)
        {
            nearExpiryBatchesQuery = nearExpiryBatchesQuery.Where(
                x => x.Warehouse.BranchID == filter.BranchID.Value);
        }

        var nearExpiryBatches = await nearExpiryBatchesQuery
            .Select(x => x.BatchID)
            .Distinct()
            .CountAsync(cancellationToken);

        var kpis = new[]
        {
            new DashboardKpiDto("sales", "Sales", sales, "BDT", null),
            new DashboardKpiDto("collections", "Collections", collections, "BDT", null),
            new DashboardKpiDto("deliveries", "Delivered Orders", delivered, null, null),
            new DashboardKpiDto("clients", "Active Clients", activeClients, null, null)
        };

        return new ExecutiveDashboardDto(
            kpis,
            await GetLeadToOrderFunnelAsync(filter, cancellationToken),
            await GetSalesTrendAsync(filter, cancellationToken),
            pendingApprovals,
            overdueTasks,
            nearExpiryBatches);
    }

    public async Task<IReadOnlyCollection<FunnelPointDto>> GetLeadToOrderFunnelAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var leads = ApplyLeadFilters(filter);
        var opportunities = ApplyOpportunityFilters(filter);
        var quotations = ApplyQuotationFilters(filter);
        var orders = ApplyOrderFilters(filter);

        var leadCount = await leads.CountAsync(cancellationToken);
        var leadValue = await leads.SumAsync(
            x => (decimal?)x.EstimatedValue,
            cancellationToken) ?? 0m;

        var opportunityCount = await opportunities.CountAsync(
            cancellationToken);
        var opportunityValue = await opportunities.SumAsync(
            x => (decimal?)x.ExpectedValue,
            cancellationToken) ?? 0m;

        var quotationCount = await quotations.CountAsync(
            cancellationToken);
        var quotationValue = await quotations.SumAsync(
            x => (decimal?)x.NetAmount,
            cancellationToken) ?? 0m;

        var orderCount = await orders.CountAsync(cancellationToken);
        var orderValue = await orders.SumAsync(
            x => (decimal?)x.NetAmount,
            cancellationToken) ?? 0m;

        return new[]
        {
            new FunnelPointDto("Lead", leadCount, leadValue),
            new FunnelPointDto("Opportunity", opportunityCount, opportunityValue),
            new FunnelPointDto("Quotation", quotationCount, quotationValue),
            new FunnelPointDto("Order", orderCount, orderValue)
        };
    }

    public async Task<IReadOnlyCollection<SeriesPointDto>> GetSalesTrendAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        return await ApplyOrderFilters(filter)
            .Where(x =>
                x.Status != OrderStatus.Rejected &&
                x.Status != OrderStatus.Cancelled)
            .GroupBy(x => x.OrderDate.Date)
            .OrderBy(x => x.Key)
            .Select(x => new SeriesPointDto(
                x.Key,
                x.Sum(y => y.NetAmount),
                "Sales"))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CampaignRoiDto>> GetCampaignRoiAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var fromDate = DateOnly.FromDateTime(filter.From);
        var toDate = DateOnly.FromDateTime(filter.To);

        var campaignsQuery = _db.Campaigns
            .AsNoTracking()
            .Where(x => x.StartDate <= toDate && x.EndDate >= fromDate);

        if (filter.CampaignID.HasValue)
        {
            campaignsQuery = campaignsQuery.Where(
                x => x.CampaignID == filter.CampaignID.Value);
        }

        if (filter.BranchID.HasValue ||
            filter.RegionID.HasValue ||
            filter.EmployeeID.HasValue)
        {
            var eligibleCampaignIDs = ApplyOrderFilters(filter)
                .Where(x => x.CampaignID.HasValue)
                .Select(x => x.CampaignID!.Value)
                .Distinct();

            campaignsQuery = campaignsQuery.Where(
                x => eligibleCampaignIDs.Contains(x.CampaignID));
        }

        var campaigns = await campaignsQuery
            .OrderBy(x => x.CampaignTitle)
            .Select(x => new
            {
                x.CampaignID,
                x.CampaignCode,
                x.CampaignTitle,
                x.Budget
            })
            .ToListAsync(cancellationToken);

        var campaignIDs = campaigns.Select(x => x.CampaignID).ToArray();

        if (campaignIDs.Length == 0)
            return Array.Empty<CampaignRoiDto>();

        var expenses = await _db.CampaignExpenses
            .AsNoTracking()
            .Where(x =>
                campaignIDs.Contains(x.CampaignID) &&
                x.ExpenseDate >= fromDate &&
                x.ExpenseDate <= toDate &&
                (x.Status == CampaignExpenseStatus.Approved ||
                 x.Status == CampaignExpenseStatus.Posted))
            .GroupBy(x => x.CampaignID)
            .Select(x => new
            {
                CampaignID = x.Key,
                Value = x.Sum(y => y.Amount)
            })
            .ToDictionaryAsync(x => x.CampaignID, x => x.Value, cancellationToken);

        var attributions = await _db.CampaignAttributions
            .AsNoTracking()
            .Where(x =>
                campaignIDs.Contains(x.CampaignID) &&
                x.CreatedAt >= filter.From &&
                x.CreatedAt <= filter.To)
            .GroupBy(x => x.CampaignID)
            .Select(x => new
            {
                CampaignID = x.Key,
                Value = x.Sum(y => y.AttributedAmount ?? 0m)
            })
            .ToDictionaryAsync(x => x.CampaignID, x => x.Value, cancellationToken);

        var deliveredValues = await ApplyOrderFilters(filter)
            .Where(x =>
                x.CampaignID.HasValue &&
                campaignIDs.Contains(x.CampaignID.Value) &&
                (x.Status == OrderStatus.PartiallyDelivered ||
                 x.Status == OrderStatus.Delivered ||
                 x.Status == OrderStatus.Closed))
            .GroupBy(x => x.CampaignID!.Value)
            .Select(x => new
            {
                CampaignID = x.Key,
                Value = x.Sum(y => y.NetAmount)
            })
            .ToDictionaryAsync(x => x.CampaignID, x => x.Value, cancellationToken);

        return campaigns
            .Select(x =>
            {
                var expense = expenses.GetValueOrDefault(x.CampaignID);
                var attributed = attributions.GetValueOrDefault(x.CampaignID);
                var delivered = deliveredValues.GetValueOrDefault(x.CampaignID);
                var roi = expense <= 0m
                    ? 0m
                    : Math.Round(((delivered - expense) / expense) * 100m, 2);

                return new CampaignRoiDto(
                    x.CampaignID,
                    x.CampaignCode,
                    x.CampaignTitle,
                    x.Budget,
                    expense,
                    attributed,
                    delivered,
                    roi);
            })
            .ToArray();
    }

    public async Task<IReadOnlyCollection<ChannelSalesDto>> GetChannelSalesAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        return await ApplyOrderFilters(filter)
            .Where(x =>
                x.Status != OrderStatus.Rejected &&
                x.Status != OrderStatus.Cancelled)
            .GroupBy(x => x.Channel)
            .OrderBy(x => x.Key)
            .Select(x => new ChannelSalesDto(
                x.Key,
                x.Count(),
                x.Sum(y => y.GrossAmount),
                x.Sum(y => y.NetAmount),
                x.Sum(y =>
                    y.Status == OrderStatus.PartiallyDelivered ||
                    y.Status == OrderStatus.Delivered ||
                    y.Status == OrderStatus.Closed
                        ? y.NetAmount
                        : 0m)))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SellInSellOutPointDto>> GetSellInSellOutAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var orderIDs = ApplyOrderFilters(filter)
            .Select(x => x.OrderID);

        var sellIn = await _db.DeliveryItems
            .AsNoTracking()
            .Where(x =>
                orderIDs.Contains(x.Delivery.OrderID) &&
                x.Delivery.DeliveredAt.HasValue &&
                x.Delivery.DeliveredAt.Value >= filter.From &&
                x.Delivery.DeliveredAt.Value <= filter.To &&
                (x.Delivery.Status == DeliveryStatus.PartiallyDelivered ||
                 x.Delivery.Status == DeliveryStatus.Delivered))
            .GroupBy(x => x.Delivery.DeliveredAt!.Value.Date)
            .Select(x => new
            {
                Period = x.Key,
                Quantity = x.Sum(y => y.QuantityDelivered),
                Value = x.Sum(y =>
                    y.QuantityDelivered * y.OrderItem.UnitPrice)
            })
            .ToListAsync(cancellationToken);

        var fromDate = DateOnly.FromDateTime(filter.From);
        var toDate = DateOnly.FromDateTime(filter.To);

        var sellOutQuery = _db.BPSellOuts
            .AsNoTracking()
            .Where(x =>
                x.SellOutDate >= fromDate &&
                x.SellOutDate <= toDate &&
                x.VerificationStatus == VerificationStatus.Verified);

        if (filter.BranchID.HasValue)
        {
            sellOutQuery = sellOutQuery.Where(
                x => x.Employee.BranchID == filter.BranchID.Value);
        }

        if (filter.RegionID.HasValue)
        {
            sellOutQuery = sellOutQuery.Where(
                x => x.Client.RegionID == filter.RegionID.Value ||
                     x.Employee.RegionID == filter.RegionID.Value);
        }

        if (filter.EmployeeID.HasValue)
        {
            sellOutQuery = sellOutQuery.Where(
                x => x.EmployeeID == filter.EmployeeID.Value);
        }

        if (filter.CampaignID.HasValue)
        {
            sellOutQuery = sellOutQuery.Where(
                x => x.CampaignID == filter.CampaignID.Value);
        }

        var sellOut = await sellOutQuery
            .GroupBy(x => x.SellOutDate)
            .Select(x => new
            {
                Period = x.Key,
                Quantity = x.Sum(y => y.TotalQuantity),
                Value = x.Sum(y => y.TotalValue)
            })
            .ToListAsync(cancellationToken);

        var sellInByDate = sellIn.ToDictionary(x => x.Period.Date);
        var sellOutByDate = sellOut.ToDictionary(
            x => x.Period.ToDateTime(TimeOnly.MinValue).Date);

        var dates = sellInByDate.Keys
            .Union(sellOutByDate.Keys)
            .OrderBy(x => x)
            .ToArray();

        return dates
            .Select(date =>
            {
                sellInByDate.TryGetValue(date, out var inValue);
                sellOutByDate.TryGetValue(date, out var outValue);

                return new SellInSellOutPointDto(
                    date,
                    inValue?.Quantity ?? 0m,
                    inValue?.Value ?? 0m,
                    outValue?.Quantity ?? 0m,
                    outValue?.Value ?? 0m);
            })
            .ToArray();
    }

    public async Task<InventoryHealthDto> GetInventoryHealthAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var branchID = filter.BranchID;

        if (!branchID.HasValue && filter.EmployeeID.HasValue)
        {
            branchID = await _db.Employees
                .AsNoTracking()
                .Where(x => x.EmployeeID == filter.EmployeeID.Value)
                .Select(x => (int?)x.BranchID)
                .SingleOrDefaultAsync(cancellationToken);
        }

        var balances = _db.StockBalances.AsNoTracking();

        if (branchID.HasValue)
        {
            balances = balances.Where(
                x => x.Warehouse.BranchID == branchID.Value);
        }

        var onHand = await balances.SumAsync(
            x => (decimal?)x.OnHandQuantity,
            cancellationToken) ?? 0m;

        var reserved = await balances.SumAsync(
            x => (decimal?)x.ReservedQuantity,
            cancellationToken) ?? 0m;

        var quarantine = await balances.SumAsync(
            x => (decimal?)x.QuarantineQuantity,
            cancellationToken) ?? 0m;

        var damaged = await balances.SumAsync(
            x => (decimal?)x.DamagedQuantity,
            cancellationToken) ?? 0m;

        var available = onHand - reserved - quarantine - damaged;
        var asOf = filter.To.Date;
        var nearExpiryEnd = asOf.AddDays(30);

        var nearExpiryBatchCount = await balances
            .Where(x =>
                x.BatchID.HasValue &&
                x.Batch!.ExpiryDate.HasValue &&
                x.Batch.ExpiryDate.Value >= asOf &&
                x.Batch.ExpiryDate.Value <= nearExpiryEnd &&
                x.OnHandQuantity > 0)
            .Select(x => x.BatchID)
            .Distinct()
            .CountAsync(cancellationToken);

        var expiredBatchCount = await balances
            .Where(x =>
                x.BatchID.HasValue &&
                x.Batch!.ExpiryDate.HasValue &&
                x.Batch.ExpiryDate.Value < asOf &&
                x.OnHandQuantity > 0)
            .Select(x => x.BatchID)
            .Distinct()
            .CountAsync(cancellationToken);

        var lowStockSkuCount = await balances
            .GroupBy(x => x.SKUID)
            .Where(x => x.Sum(y =>
                y.OnHandQuantity -
                y.ReservedQuantity -
                y.QuarantineQuantity -
                y.DamagedQuantity) <= LowStockThreshold)
            .CountAsync(cancellationToken);

        var rows = await balances
            .Select(x => new
            {
                x.WarehouseID,
                x.Warehouse.WarehouseName,
                x.SKUID,
                x.SKU.SKUCode,
                x.SKU.SKUName,
                x.BatchID,
                BatchNo = x.Batch == null ? null : x.Batch.BatchNo,
                ExpiryDate = x.Batch == null ? null : x.Batch.ExpiryDate,
                BatchStatus = x.Batch == null
                    ? (BatchStatus?)null
                    : x.Batch.Status,
                x.OnHandQuantity,
                x.ReservedQuantity,
                x.QuarantineQuantity,
                x.DamagedQuantity
            })
            .ToListAsync(cancellationToken);

        var items = rows
            .Select(x =>
            {
                var rowAvailable =
                    x.OnHandQuantity -
                    x.ReservedQuantity -
                    x.QuarantineQuantity -
                    x.DamagedQuantity;

                var isNearExpiry =
                    x.ExpiryDate.HasValue &&
                    x.ExpiryDate.Value >= asOf &&
                    x.ExpiryDate.Value <= nearExpiryEnd &&
                    x.OnHandQuantity > 0;

                return new InventoryHealthItemDto(
                    x.WarehouseID,
                    x.WarehouseName,
                    x.SKUID,
                    x.SKUCode,
                    x.SKUName,
                    x.BatchID,
                    x.BatchNo,
                    x.ExpiryDate,
                    x.BatchStatus,
                    x.OnHandQuantity,
                    x.ReservedQuantity,
                    x.QuarantineQuantity,
                    x.DamagedQuantity,
                    rowAvailable,
                    rowAvailable <= LowStockThreshold,
                    isNearExpiry);
            })
            .Where(x =>
                x.IsLowStock ||
                x.IsNearExpiry ||
                x.QuarantineQuantity > 0 ||
                x.DamagedQuantity > 0)
            .OrderByDescending(x => x.IsNearExpiry)
            .ThenBy(x => x.AvailableQuantity)
            .Take(50)
            .ToArray();

        return new InventoryHealthDto(
            onHand,
            available,
            reserved,
            quarantine,
            damaged,
            nearExpiryBatchCount,
            expiredBatchCount,
            lowStockSkuCount,
            LowStockThreshold,
            items);
    }

    public async Task<IReadOnlyCollection<DeliveryReturnPointDto>> GetDeliveryReturnAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var orderIDs = ApplyOrderFilters(filter)
            .Select(x => x.OrderID);

        var deliveries = await _db.Deliveries
            .AsNoTracking()
            .Where(x =>
                orderIDs.Contains(x.OrderID) &&
                ((x.PlannedDeliveryDate.HasValue &&
                  x.PlannedDeliveryDate.Value >= filter.From &&
                  x.PlannedDeliveryDate.Value <= filter.To) ||
                 (x.DeliveredAt.HasValue &&
                  x.DeliveredAt.Value >= filter.From &&
                  x.DeliveredAt.Value <= filter.To) ||
                 (x.DispatchDate.HasValue &&
                  x.DispatchDate.Value >= filter.From &&
                  x.DispatchDate.Value <= filter.To)))
            .Select(x => new
            {
                Date = (x.DeliveredAt ??
                        x.DispatchDate ??
                        x.PlannedDeliveryDate ??
                        x.CreatedAt).Date,
                x.Status
            })
            .ToListAsync(cancellationToken);

        var returns = await _db.ReturnRequests
            .AsNoTracking()
            .Where(x =>
                orderIDs.Contains(x.OrderID) &&
                x.RequestDate >= filter.From &&
                x.RequestDate <= filter.To)
            .Select(x => new
            {
                Date = x.RequestDate.Date,
                x.ReturnRequestID
            })
            .ToListAsync(cancellationToken);

        var returnIDs = returns.Select(x => x.ReturnRequestID).ToArray();

        var returnedQuantities = returnIDs.Length == 0
            ? new Dictionary<int, decimal>()
            : await _db.ReturnItems
                .AsNoTracking()
                .Where(x => returnIDs.Contains(x.ReturnRequestID))
                .GroupBy(x => x.ReturnRequestID)
                .Select(x => new
                {
                    ReturnRequestID = x.Key,
                    Quantity = x.Sum(y => y.ReceivedQuantity)
                })
                .ToDictionaryAsync(
                    x => x.ReturnRequestID,
                    x => x.Quantity,
                    cancellationToken);

        var dates = deliveries
            .Select(x => x.Date)
            .Union(returns.Select(x => x.Date))
            .OrderBy(x => x)
            .ToArray();

        return dates
            .Select(date =>
            {
                var typedDeliveries = deliveries
                    .Where(x => x.Date == date)
                    .ToArray();
                var typedReturns = returns
                    .Where(x => x.Date == date)
                    .ToArray();

                return new DeliveryReturnPointDto(
                    date,
                    typedDeliveries.Length,
                    typedDeliveries.Count(x =>
                        x.Status == DeliveryStatus.Delivered),
                    typedDeliveries.Count(x =>
                        x.Status == DeliveryStatus.PartiallyDelivered),
                    typedDeliveries.Count(x =>
                        x.Status == DeliveryStatus.Failed),
                    typedDeliveries.Count(x =>
                        x.Status == DeliveryStatus.Rescheduled),
                    typedReturns.Length,
                    typedReturns.Sum(x =>
                        returnedQuantities.GetValueOrDefault(
                            x.ReturnRequestID)));
            })
            .ToArray();
    }

    public async Task<IReadOnlyCollection<EmployeeKpiDto>> GetEmployeeKpiAsync(
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var targets = _db.EmployeeTargets
            .AsNoTracking()
            .Where(x =>
                x.TargetPeriodStart <= filter.To &&
                x.TargetPeriodEnd >= filter.From &&
                x.Status != EmployeeTargetStatus.Cancelled);

        if (filter.BranchID.HasValue)
        {
            targets = targets.Where(
                x => x.Employee.BranchID == filter.BranchID.Value);
        }

        if (filter.RegionID.HasValue)
        {
            targets = targets.Where(
                x => x.Employee.RegionID == filter.RegionID.Value);
        }

        if (filter.EmployeeID.HasValue)
        {
            targets = targets.Where(
                x => x.EmployeeID == filter.EmployeeID.Value);
        }

        if (filter.CampaignID.HasValue)
        {
            targets = targets.Where(
                x => x.CampaignID == filter.CampaignID.Value);
        }

        var targetRows = await targets
            .GroupBy(x => x.EmployeeID)
            .Select(x => new
            {
                EmployeeID = x.Key,
                TargetValue = x.Sum(y =>
                    y.TargetAmount ?? y.TargetValue)
            })
            .ToListAsync(cancellationToken);

        var rewards = _db.RewardCalculations
            .AsNoTracking()
            .Where(x =>
                x.PeriodStart <= filter.To &&
                x.PeriodEnd >= filter.From &&
                x.Status != RewardCalculationStatus.Rejected);

        if (filter.BranchID.HasValue)
        {
            rewards = rewards.Where(
                x => x.Employee.BranchID == filter.BranchID.Value);
        }

        if (filter.RegionID.HasValue)
        {
            rewards = rewards.Where(
                x => x.Employee.RegionID == filter.RegionID.Value);
        }

        if (filter.EmployeeID.HasValue)
        {
            rewards = rewards.Where(
                x => x.EmployeeID == filter.EmployeeID.Value);
        }

        if (filter.CampaignID.HasValue)
        {
            rewards = rewards.Where(
                x => x.EmployeeTarget != null &&
                     x.EmployeeTarget.CampaignID == filter.CampaignID.Value);
        }

        var rewardRows = await rewards
            .GroupBy(x => x.EmployeeID)
            .Select(x => new
            {
                EmployeeID = x.Key,
                ActualValue = x.Sum(y => y.ActualValue),
                RewardAmount = x.Sum(y => y.FinalAmount)
            })
            .ToListAsync(cancellationToken);

        var employeeIDs = targetRows
            .Select(x => x.EmployeeID)
            .Union(rewardRows.Select(x => x.EmployeeID))
            .Distinct()
            .ToArray();

        var employees = await _db.Employees
            .AsNoTracking()
            .Where(x => employeeIDs.Contains(x.EmployeeID))
            .Select(x => new
            {
                x.EmployeeID,
                x.EmployeeCode,
                EmployeeName = x.User != null
                    ? x.User.FullName
                    : x.EmployeeCode
            })
            .ToDictionaryAsync(x => x.EmployeeID, cancellationToken);

        var targetByEmployee = targetRows.ToDictionary(
            x => x.EmployeeID,
            x => x.TargetValue);
        var rewardByEmployee = rewardRows.ToDictionary(
            x => x.EmployeeID);

        return employeeIDs
            .Where(employees.ContainsKey)
            .Select(employeeID =>
            {
                var employee = employees[employeeID];
                var target = targetByEmployee.GetValueOrDefault(employeeID);
                rewardByEmployee.TryGetValue(employeeID, out var reward);
                var actual = reward?.ActualValue ?? 0m;
                var achievement = target <= 0m
                    ? 0m
                    : Math.Round((actual / target) * 100m, 2);

                return new EmployeeKpiDto(
                    employeeID,
                    employee.EmployeeCode,
                    employee.EmployeeName,
                    target,
                    actual,
                    achievement,
                    reward?.RewardAmount ?? 0m);
            })
            .OrderByDescending(x => x.AchievementPercent)
            .ThenBy(x => x.EmployeeName)
            .ToArray();
    }

    public async Task<Client360Dto> GetClient360Async(
        int clientID,
        AnalyticsFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var client = await _db.Clients
            .AsNoTracking()
            .Where(x => x.ClientID == clientID)
            .Select(x => new
            {
                x.ClientID,
                x.ClientCode,
                x.ClientName,
                x.ClientType,
                x.Channel,
                x.Phone,
                x.Email,
                x.Address,
                x.LifecycleStatus,
                x.RiskStatus
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Client was not found.");

        var credit = await _db.ClientCreditProfiles
            .AsNoTracking()
            .Where(x => x.ClientID == clientID)
            .Select(x => new
            {
                x.CreditLimit,
                x.CurrentDue,
                x.IsBlocked
            })
            .SingleOrDefaultAsync(cancellationToken);

        var orders = await _db.Orders
            .AsNoTracking()
            .Where(x =>
                x.ClientID == clientID &&
                x.OrderDate >= filter.From &&
                x.OrderDate <= filter.To)
            .OrderByDescending(x => x.OrderDate)
            .Select(x => new Client360OrderDto(
                x.OrderID,
                x.OrderNo,
                x.OrderDate,
                x.Channel,
                x.Status,
                x.NetAmount))
            .ToListAsync(cancellationToken);

        var payments = await _db.Payments
            .AsNoTracking()
            .Where(x =>
                x.ClientID == clientID &&
                x.PaymentDate >= filter.From &&
                x.PaymentDate <= filter.To)
            .OrderByDescending(x => x.PaymentDate)
            .Select(x => new Client360PaymentDto(
                x.PaymentID,
                x.PaymentNo,
                x.PaymentDate,
                x.PaymentMethod,
                x.Status,
                x.Amount))
            .ToListAsync(cancellationToken);

        var complaints = await _db.Complaints
            .AsNoTracking()
            .Where(x =>
                x.ClientID == clientID &&
                x.OpenedAt >= filter.From &&
                x.OpenedAt <= filter.To)
            .OrderByDescending(x => x.OpenedAt)
            .Select(x => new Client360ComplaintDto(
                x.ComplaintID,
                x.ComplaintNo,
                x.OpenedAt,
                x.Priority,
                x.Status,
                x.Subject))
            .ToListAsync(cancellationToken);

        var activities = await _db.CRMActivities
            .AsNoTracking()
            .Where(x =>
                x.ClientID == clientID &&
                x.ActivityAt >= filter.From &&
                x.ActivityAt <= filter.To)
            .OrderByDescending(x => x.ActivityAt)
            .Take(20)
            .Select(x => new
            {
                x.CRMActivityID,
                x.ActivityAt,
                x.Subject,
                x.ActivityStatus
            })
            .ToListAsync(cancellationToken);

        var visits = await _db.Visits
            .AsNoTracking()
            .Where(x =>
                x.ClientID == clientID &&
                x.CheckInAt >= filter.From &&
                x.CheckInAt <= filter.To)
            .OrderByDescending(x => x.CheckInAt)
            .Take(20)
            .Select(x => new
            {
                x.VisitID,
                x.CheckInAt,
                x.VisitType,
                x.Status
            })
            .ToListAsync(cancellationToken);

        var orderValue = orders.Sum(x => x.NetAmount);
        var paidAmount = payments
            .Where(x => x.Status == CustomerPaymentStatus.Confirmed)
            .Sum(x => x.Amount);
        var openComplaintCount = complaints.Count(x =>
            x.Status != ComplaintStatus.Resolved &&
            x.Status != ComplaintStatus.Closed);

        var header = new Client360HeaderDto(
            client.ClientID,
            client.ClientCode,
            client.ClientName,
            client.ClientType,
            client.Channel,
            client.Phone,
            client.Email,
            client.Address,
            client.LifecycleStatus,
            client.RiskStatus,
            credit?.CreditLimit ?? 0m,
            credit?.CurrentDue ?? 0m,
            credit?.IsBlocked ?? false,
            orders.Count,
            orderValue,
            paidAmount,
            openComplaintCount);

        var timeline = new List<Client360TimelineItemDto>();

        timeline.AddRange(orders.Select(x => new Client360TimelineItemDto(
            x.OrderDate,
            "Order",
            x.OrderNo,
            x.Status.ToString(),
            x.NetAmount,
            x.OrderID)));

        timeline.AddRange(payments.Select(x => new Client360TimelineItemDto(
            x.PaymentDate,
            "Payment",
            x.PaymentNo,
            x.Status.ToString(),
            x.Amount,
            x.PaymentID)));

        timeline.AddRange(complaints.Select(x => new Client360TimelineItemDto(
            x.OpenedAt,
            "Complaint",
            x.Subject,
            x.Status.ToString(),
            null,
            x.ComplaintID)));

        timeline.AddRange(activities.Select(x => new Client360TimelineItemDto(
            x.ActivityAt,
            "CRM Activity",
            x.Subject,
            x.ActivityStatus.ToString(),
            null,
            x.CRMActivityID)));

        timeline.AddRange(visits.Select(x => new Client360TimelineItemDto(
            x.CheckInAt,
            "Visit",
            x.VisitType.ToString(),
            x.Status.ToString(),
            null,
            x.VisitID)));

        return new Client360Dto(
            header,
            orders.Take(10).ToArray(),
            payments.Take(10).ToArray(),
            complaints.Take(10).ToArray(),
            timeline
                .OrderByDescending(x => x.OccurredAt)
                .Take(50)
                .ToArray());
    }

    private IQueryable<Order> ApplyOrderFilters(AnalyticsFilterDto filter)
    {
        var query = _db.Orders
            .AsNoTracking()
            .Where(x =>
                x.OrderDate >= filter.From &&
                x.OrderDate <= filter.To);

        if (filter.BranchID.HasValue)
        {
            query = query.Where(x =>
                x.EmployeeID.HasValue &&
                x.Employee!.BranchID == filter.BranchID.Value);
        }

        if (filter.RegionID.HasValue)
        {
            query = query.Where(x =>
                x.Client.RegionID == filter.RegionID.Value ||
                (x.EmployeeID.HasValue &&
                 x.Employee!.RegionID == filter.RegionID.Value));
        }

        if (filter.EmployeeID.HasValue)
        {
            query = query.Where(x =>
                x.EmployeeID == filter.EmployeeID.Value);
        }

        if (filter.CampaignID.HasValue)
        {
            query = query.Where(x =>
                x.CampaignID == filter.CampaignID.Value);
        }

        return query;
    }

    private IQueryable<Lead> ApplyLeadFilters(AnalyticsFilterDto filter)
    {
        var query = _db.Leads
            .AsNoTracking()
            .Where(x =>
                x.CreatedAt >= filter.From &&
                x.CreatedAt <= filter.To);

        if (filter.BranchID.HasValue)
        {
            query = query.Where(x =>
                x.AssignedEmployeeID.HasValue &&
                x.AssignedEmployee!.BranchID == filter.BranchID.Value);
        }

        if (filter.RegionID.HasValue)
        {
            query = query.Where(x =>
                x.RegionID == filter.RegionID.Value);
        }

        if (filter.EmployeeID.HasValue)
        {
            query = query.Where(x =>
                x.AssignedEmployeeID == filter.EmployeeID.Value);
        }

        if (filter.CampaignID.HasValue)
        {
            query = query.Where(x =>
                x.SourceCampaignID == filter.CampaignID.Value);
        }

        return query;
    }

    private IQueryable<Opportunity> ApplyOpportunityFilters(
        AnalyticsFilterDto filter)
    {
        var query = _db.Opportunities
            .AsNoTracking()
            .Where(x =>
                x.CreatedAt >= filter.From &&
                x.CreatedAt <= filter.To);

        if (filter.BranchID.HasValue)
        {
            query = query.Where(x =>
                x.OwnerEmployee.BranchID == filter.BranchID.Value);
        }

        if (filter.RegionID.HasValue)
        {
            query = query.Where(x =>
                x.OwnerEmployee.RegionID == filter.RegionID.Value ||
                (x.ClientID.HasValue &&
                 x.Client!.RegionID == filter.RegionID.Value));
        }

        if (filter.EmployeeID.HasValue)
        {
            query = query.Where(x =>
                x.OwnerEmployeeID == filter.EmployeeID.Value);
        }

        if (filter.CampaignID.HasValue)
        {
            query = query.Where(x =>
                x.CampaignID == filter.CampaignID.Value);
        }

        return query;
    }

    private IQueryable<Quotation> ApplyQuotationFilters(
        AnalyticsFilterDto filter)
    {
        var query = _db.Quotations
            .AsNoTracking()
            .Where(x =>
                x.CreatedAt >= filter.From &&
                x.CreatedAt <= filter.To);

        if (filter.BranchID.HasValue)
        {
            query = query.Where(x =>
                x.Opportunity != null &&
                x.Opportunity.OwnerEmployee.BranchID ==
                    filter.BranchID.Value);
        }

        if (filter.RegionID.HasValue)
        {
            query = query.Where(x =>
                x.Client.RegionID == filter.RegionID.Value ||
                (x.Opportunity != null &&
                 x.Opportunity.OwnerEmployee.RegionID ==
                    filter.RegionID.Value));
        }

        if (filter.EmployeeID.HasValue)
        {
            query = query.Where(x =>
                x.Opportunity != null &&
                x.Opportunity.OwnerEmployeeID == filter.EmployeeID.Value);
        }

        if (filter.CampaignID.HasValue)
        {
            query = query.Where(x =>
                x.CampaignID == filter.CampaignID.Value);
        }

        return query;
    }

    private static bool HasEmployeeDrivenFilter(AnalyticsFilterDto filter) =>
        filter.BranchID.HasValue ||
        filter.EmployeeID.HasValue ||
        filter.CampaignID.HasValue;
}

