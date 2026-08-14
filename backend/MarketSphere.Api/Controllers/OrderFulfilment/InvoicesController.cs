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
[Route("api/invoices")]
public sealed class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoicesController(IInvoiceService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.InvoicesView)]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<InvoiceListDto>>.Success(result, "Invoices retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.InvoicesView)]
    public async Task<ActionResult<ApiResponse<InvoiceDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<InvoiceDetailsDto>.Success(result, "Invoice retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.InvoicesManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateFromOrder(
        [FromBody] CreateInvoiceRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateFromOrderAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Invoice created successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [HasPermission(PermissionCodes.InvoicesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int id,
        [FromBody] ChangeInvoiceStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Invoice status changed successfully."));
    }
}
