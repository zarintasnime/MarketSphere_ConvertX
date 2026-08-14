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
[Route("api/purchase-invoices")]
public sealed class PurchaseInvoicesController : ControllerBase
{
    private readonly IPurchaseInvoiceService _service;

    public PurchaseInvoicesController(IPurchaseInvoiceService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.PurchaseInvoicesView)]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseInvoiceDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<PurchaseInvoiceDto>>.Success(result, "Purchase invoices retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.PurchaseInvoicesManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SavePurchaseInvoiceRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Purchase invoice created successfully."));
    }

    [HttpPost("{id:int}/confirm")]
    [HasPermission(PermissionCodes.PurchaseInvoicesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Confirm(
        int id,
        CancellationToken cancellationToken)
    {
        await _service.ConfirmAsync(id, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Purchase invoice confirmed successfully."));
    }

    [HttpPost("payments")]
    [HasPermission(PermissionCodes.SupplierPaymentsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreatePayment(
        [FromBody] CreateSupplierPaymentRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreatePaymentAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Supplier payment created successfully."));
    }

    [HttpPatch("payments/{supplierPaymentID:int}/status")]
    [HasPermission(PermissionCodes.SupplierPaymentsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePaymentStatus(
        int supplierPaymentID,
        [FromBody] ChangeSupplierPaymentStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangePaymentStatusAsync(supplierPaymentID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Supplier payment status changed successfully."));
    }

    [HttpGet("{purchaseInvoiceID:int}/payments")]
    [HasPermission(PermissionCodes.PurchaseInvoicesView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<SupplierPaymentDto>>>> GetPayments(
            int purchaseInvoiceID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPaymentsAsync(purchaseInvoiceID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<SupplierPaymentDto>>.Success(result, "Supplier payments retrieved successfully."));
    }
}
