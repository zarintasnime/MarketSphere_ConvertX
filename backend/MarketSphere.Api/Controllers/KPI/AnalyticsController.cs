using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.KPI.DTOs;
using MarketSphere.Application.Modules.KPI.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.KPI;

[ApiController]
[Authorize]
[Route("api/analytics")]
public sealed class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _service;

    public AnalyticsController(IAnalyticsService service)
    {
        _service = service;
    }

    [HttpGet("executive-dashboard")]
    [HasPermission(PermissionCodes.AnalyticsView)]
    public async Task<ActionResult<ApiResponse<ExecutiveDashboardDto>>> GetExecutiveDashboard(
        [FromQuery] AnalyticsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetExecutiveDashboardAsync(
            filter,
            cancellationToken);

        return Ok(ApiResponse<ExecutiveDashboardDto>.Success(
            result,
            "Executive dashboard retrieved successfully."));
    }

    [HttpGet("lead-to-order-funnel")]
    [HasPermission(PermissionCodes.AnalyticsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<FunnelPointDto>>>> GetLeadToOrderFunnel(
        [FromQuery] AnalyticsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetLeadToOrderFunnelAsync(
            filter,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<FunnelPointDto>>.Success(
            result,
            "Lead-to-order funnel retrieved successfully."));
    }

    [HttpGet("sales-trend")]
    [HasPermission(PermissionCodes.AnalyticsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<SeriesPointDto>>>> GetSalesTrend(
        [FromQuery] AnalyticsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetSalesTrendAsync(
            filter,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<SeriesPointDto>>.Success(
            result,
            "Sales trend retrieved successfully."));
    }

    [HttpGet("campaign-roi")]
    [HasPermission(PermissionCodes.AnalyticsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CampaignRoiDto>>>> GetCampaignRoi(
        [FromQuery] AnalyticsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetCampaignRoiAsync(
            filter,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<CampaignRoiDto>>.Success(
            result,
            "Campaign ROI analytics retrieved successfully."));
    }

    [HttpGet("channel-sales")]
    [HasPermission(PermissionCodes.AnalyticsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ChannelSalesDto>>>> GetChannelSales(
        [FromQuery] AnalyticsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetChannelSalesAsync(
            filter,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<ChannelSalesDto>>.Success(
            result,
            "Channel sales analytics retrieved successfully."));
    }

    [HttpGet("sell-in-sell-out")]
    [HasPermission(PermissionCodes.AnalyticsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<SellInSellOutPointDto>>>> GetSellInSellOut(
        [FromQuery] AnalyticsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetSellInSellOutAsync(
            filter,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<SellInSellOutPointDto>>.Success(
            result,
            "Sell-in and sell-out analytics retrieved successfully."));
    }

    [HttpGet("inventory-health")]
    [HasPermission(PermissionCodes.AnalyticsView)]
    public async Task<ActionResult<ApiResponse<InventoryHealthDto>>> GetInventoryHealth(
        [FromQuery] AnalyticsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetInventoryHealthAsync(
            filter,
            cancellationToken);

        return Ok(ApiResponse<InventoryHealthDto>.Success(
            result,
            "Inventory health analytics retrieved successfully."));
    }

    [HttpGet("delivery-return")]
    [HasPermission(PermissionCodes.AnalyticsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<DeliveryReturnPointDto>>>> GetDeliveryReturn(
        [FromQuery] AnalyticsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetDeliveryReturnAsync(
            filter,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<DeliveryReturnPointDto>>.Success(
            result,
            "Delivery and return analytics retrieved successfully."));
    }

    [HttpGet("employee-kpi")]
    [HasPermission(PermissionCodes.AnalyticsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<EmployeeKpiDto>>>> GetEmployeeKpi(
        [FromQuery] AnalyticsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetEmployeeKpiAsync(
            filter,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<EmployeeKpiDto>>.Success(
            result,
            "Employee KPI analytics retrieved successfully."));
    }

    [HttpGet("client-360/{clientID:int}")]
    [HasPermission(PermissionCodes.AnalyticsView)]
    public async Task<ActionResult<ApiResponse<Client360Dto>>> GetClient360(
        int clientID,
        [FromQuery] AnalyticsFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetClient360Async(
            clientID,
            filter,
            cancellationToken);

        return Ok(ApiResponse<Client360Dto>.Success(
            result,
            "Client 360 analytics retrieved successfully."));
    }
}
