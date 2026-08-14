using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
using MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.OrderFulfilment;

[ApiController]
[Authorize]
[Route("api/mt-purchase-orders")]
public sealed class ModernTradePurchaseOrdersController : ControllerBase
{
    private readonly IModernTradePurchaseOrderService _service;

    public ModernTradePurchaseOrdersController(IModernTradePurchaseOrderService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.ModernTradePurchaseOrdersView)]
    public async Task<ActionResult<ApiResponse<PagedResult<ModernTradePurchaseOrderListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<ModernTradePurchaseOrderListDto>>.Success(result, "Modern trade purchase orders retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.ModernTradePurchaseOrdersView)]
    public async Task<ActionResult<ApiResponse<ModernTradePurchaseOrderDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ModernTradePurchaseOrderDetailsDto>.Success(result, "Modern trade purchase order retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.ModernTradePurchaseOrdersManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveModernTradePurchaseOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Modern trade purchase order created successfully."));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.ModernTradePurchaseOrdersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateDraft(
        int id,
        [FromBody] SaveModernTradePurchaseOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateDraftAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Modern trade purchase order updated successfully."));
    }

    [HttpPut("items/{itemID:int}/mapping")]
    [HasPermission(PermissionCodes.ModernTradePurchaseOrdersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> MapItem(
        int itemID,
        [FromBody] MapModernTradePurchaseOrderItemRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.MapItemAsync(itemID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Modern trade purchase order item mapped successfully."));
    }

    [HttpPost("{id:int}/submit")]
    [HasPermission(PermissionCodes.ModernTradePurchaseOrdersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Submit(
        int id,
        CancellationToken cancellationToken)
    {
        await _service.SubmitAsync(id, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Modern trade purchase order submitted successfully."));
    }

    [HttpPost("{id:int}/verify")]
    [HasPermission(PermissionCodes.ModernTradePurchaseOrdersVerify)]
    public async Task<ActionResult<ApiResponse<bool>>> Verify(
        int id,
        [FromBody] VerifyModernTradePurchaseOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.VerifyAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Modern trade purchase order verified successfully."));
    }
}
