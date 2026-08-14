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
[Route("api/returns")]
public sealed class ReturnsController : ControllerBase
{
    private readonly IReturnService _service;

    public ReturnsController(IReturnService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.ReturnsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<ReturnListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<ReturnListDto>>.Success(result, "Returns retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.ReturnsView)]
    public async Task<ActionResult<ApiResponse<ReturnDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ReturnDetailsDto>.Success(result, "Return retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.ReturnsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreateReturnRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Return request created successfully."));
    }

    [HttpPost("{id:int}/approve")]
    [HasPermission(PermissionCodes.ReturnsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Approve(
        int id,
        [FromBody] ApproveReturnRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ApproveAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Return request approved successfully."));
    }

    [HttpPost("{id:int}/resolve")]
    [HasPermission(PermissionCodes.ReturnsResolve)]
    public async Task<ActionResult<ApiResponse<bool>>> Resolve(
        int id,
        [FromBody] ResolveReturnRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ResolveAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Return request resolved successfully."));
    }
}
