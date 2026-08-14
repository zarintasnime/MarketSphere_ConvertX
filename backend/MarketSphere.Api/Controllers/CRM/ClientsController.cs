using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.CRM.DTOs;
using MarketSphere.Application.Modules.CRM.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.CRM;

[ApiController]
[Authorize]
[Route("api/clients")]
public sealed class ClientsController : ControllerBase
{
    private readonly IClientService _service;
    private readonly ICrmDashboardService _dashboardService;

    public ClientsController(
        IClientService service,
        ICrmDashboardService dashboardService)
    {
        _service = service;
        _dashboardService = dashboardService;
    }

    [HttpGet("dashboard")]
    [HasPermission(PermissionCodes.CrmDashboardView)]
    public async Task<ActionResult<ApiResponse<CrmDashboardDto>>> GetDashboard(
            CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetAsync(cancellationToken);
        return Ok(ApiResponse<CrmDashboardDto>.Success(result, "CRM dashboard retrieved successfully."));
    }

    [HttpGet]
    [HasPermission(PermissionCodes.ClientsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<ClientListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<ClientListDto>>.Success(result, "Clients retrieved successfully."));
    }

    [HttpGet("{clientID:int}")]
    [HasPermission(PermissionCodes.ClientsView)]
    public async Task<ActionResult<ApiResponse<ClientDetailsDto>>> GetById(
            int clientID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(clientID, cancellationToken);
        return Ok(ApiResponse<ClientDetailsDto>.Success(result, "Client retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.ClientsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveClientRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Client created successfully."));
    }

    [HttpPut("{clientID:int}")]
    [HasPermission(PermissionCodes.ClientsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int clientID,
        [FromBody] SaveClientRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(clientID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Client updated successfully."));
    }

    [HttpPost("{clientID:int}/contacts")]
    [HasPermission(PermissionCodes.ClientsManage)]
    public async Task<ActionResult<ApiResponse<int>>> AddContact(
        int clientID,
        [FromBody] SaveClientContactRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.AddContactAsync(clientID, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Client contact added successfully."));
    }

    [HttpPut("contacts/{clientContactID:int}")]
    [HasPermission(PermissionCodes.ClientsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateContact(
        int clientContactID,
        [FromBody] SaveClientContactRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateContactAsync(clientContactID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Client contact updated successfully."));
    }

    [HttpPut("{clientID:int}/credit-profile")]
    [HasPermission(PermissionCodes.ClientCreditManage)]
    public async Task<ActionResult<ApiResponse<bool>>> SetCreditProfile(
        int clientID,
        [FromBody] SaveClientCreditProfileRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.SetCreditProfileAsync(clientID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Client credit profile updated successfully."));
    }

    [HttpPatch("{clientID:int}/lifecycle")]
    [HasPermission(PermissionCodes.ClientsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeLifecycle(
        int clientID,
        [FromBody] ChangeClientLifecycleRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeLifecycleAsync(clientID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Client lifecycle changed successfully."));
    }

    [HttpPost("segments")]
    [HasPermission(PermissionCodes.ClientsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateSegment(
        [FromBody] SaveClientSegmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateSegmentAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Client segment created successfully."));
    }

    [HttpPost("{clientID:int}/segments")]
    [HasPermission(PermissionCodes.ClientsManage)]
    public async Task<ActionResult<ApiResponse<int>>> AssignSegment(
        int clientID,
        [FromBody] AssignClientSegmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.AssignSegmentAsync(clientID, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Client segment assigned successfully."));
    }

    [HttpPost("segment-assignments/{clientSegmentAssignmentID:int}/end")]
    [HasPermission(PermissionCodes.ClientsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> EndSegmentAssignment(
        int clientSegmentAssignmentID,
        [FromQuery] DateTime effectiveTo,
        CancellationToken cancellationToken)
    {
        await _service.EndSegmentAssignmentAsync(clientSegmentAssignmentID, effectiveTo, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Client segment assignment ended successfully."));
    }
}
