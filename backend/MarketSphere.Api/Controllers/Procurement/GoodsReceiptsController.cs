using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Procurement.DTOs;
using MarketSphere.Application.Modules.Procurement.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.Procurement;

[ApiController]
[Authorize]
[Route("api/goods-receipts")]
public sealed class GoodsReceiptsController : ControllerBase
{
    private readonly IGoodsReceiptService _service;

    public GoodsReceiptsController(IGoodsReceiptService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.GoodsReceiptsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<GoodsReceiptListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<GoodsReceiptListDto>>.Success(result, "Goods receipt records retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.GoodsReceiptsView)]
    public async Task<ActionResult<ApiResponse<GoodsReceiptDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<GoodsReceiptDetailsDto>.Success(result, "Goods receipt retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.GoodsReceiptsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveGoodsReceiptRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Goods receipt created successfully."));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.GoodsReceiptsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id,
        [FromBody] SaveGoodsReceiptRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Goods receipt updated successfully."));
    }

    [HttpPost("{id:int}/quality-check")]
    [HasPermission(PermissionCodes.GoodsReceiptsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> CompleteQualityCheck(
        int id,
        [FromBody] CompleteQualityCheckRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.CompleteQualityCheckAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Quality check completed successfully."));
    }

    [HttpPost("{id:int}/post")]
    [HasPermission(PermissionCodes.GoodsReceiptsPost)]
    public async Task<ActionResult<ApiResponse<bool>>> Post(
        int id,
        [FromBody] PostGoodsReceiptRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.PostAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Goods receipt posted successfully."));
    }
}
