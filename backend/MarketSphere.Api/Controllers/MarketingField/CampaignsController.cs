using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.MarketingField.DTOs;
using MarketSphere.Application.Modules.MarketingField.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.MarketingField;

[ApiController]
[Authorize]
[Route("api/campaigns")]
public sealed class CampaignsController : ControllerBase
{
    private readonly ICampaignService _service;

    public CampaignsController(ICampaignService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.CampaignsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<CampaignListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<CampaignListDto>>.Success(result, "Campaigns retrieved successfully."));
    }

    [HttpGet("{campaignID:int}")]
    [HasPermission(PermissionCodes.CampaignsView)]
    public async Task<ActionResult<ApiResponse<CampaignDetailsDto>>> GetById(
            int campaignID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(campaignID, cancellationToken);
        return Ok(ApiResponse<CampaignDetailsDto>.Success(result, "Campaign retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.CampaignsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveCampaignRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Campaign created successfully."));
    }

    [HttpPut("{campaignID:int}")]
    [HasPermission(PermissionCodes.CampaignsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int campaignID,
        [FromBody] SaveCampaignRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(campaignID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Campaign updated successfully."));
    }

    [HttpPatch("{campaignID:int}/status")]
    [HasPermission(PermissionCodes.CampaignsApprove)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int campaignID,
        [FromBody] ChangeCampaignStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(campaignID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Campaign status changed successfully."));
    }

    [HttpPost("{campaignID:int}/targets")]
    [HasPermission(PermissionCodes.CampaignsManage)]
    public async Task<ActionResult<ApiResponse<int>>> AddTarget(
        int campaignID,
        [FromBody] SaveCampaignTargetRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.AddTargetAsync(campaignID, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Campaign target added successfully."));
    }

    [HttpPut("targets/{campaignTargetID:int}")]
    [HasPermission(PermissionCodes.CampaignsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateTarget(
        int campaignTargetID,
        [FromBody] SaveCampaignTargetRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateTargetAsync(campaignTargetID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Campaign target updated successfully."));
    }

    [HttpDelete("targets/{campaignTargetID:int}")]
    [HasPermission(PermissionCodes.CampaignsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTarget(
        int campaignTargetID,
        CancellationToken cancellationToken)
    {
        await _service.DeleteTargetAsync(campaignTargetID, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Campaign target deleted successfully."));
    }

    [HttpPost("{campaignID:int}/offers")]
    [HasPermission(PermissionCodes.CampaignsManage)]
    public async Task<ActionResult<ApiResponse<int>>> AddOffer(
        int campaignID,
        [FromBody] SaveCampaignOfferRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.AddOfferAsync(campaignID, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Campaign offer added successfully."));
    }

    [HttpPut("offers/{campaignOfferID:int}")]
    [HasPermission(PermissionCodes.CampaignsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateOffer(
        int campaignOfferID,
        [FromBody] SaveCampaignOfferRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateOfferAsync(campaignOfferID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Campaign offer updated successfully."));
    }

    [HttpDelete("offers/{campaignOfferID:int}")]
    [HasPermission(PermissionCodes.CampaignsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteOffer(
        int campaignOfferID,
        CancellationToken cancellationToken)
    {
        await _service.DeleteOfferAsync(campaignOfferID, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Campaign offer deleted successfully."));
    }

    [HttpPost("{campaignID:int}/expenses")]
    [HasPermission(PermissionCodes.CampaignExpensesManage)]
    public async Task<ActionResult<ApiResponse<int>>> AddExpense(
        int campaignID,
        [FromBody] SaveCampaignExpenseRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.AddExpenseAsync(campaignID, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Campaign expense added successfully."));
    }

    [HttpPut("expenses/{campaignExpenseID:int}")]
    [HasPermission(PermissionCodes.CampaignExpensesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateExpense(
        int campaignExpenseID,
        [FromBody] SaveCampaignExpenseRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateExpenseAsync(campaignExpenseID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Campaign expense updated successfully."));
    }

    [HttpPatch("expenses/{campaignExpenseID:int}/status")]
    [HasPermission(PermissionCodes.CampaignExpensesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeExpenseStatus(
        int campaignExpenseID,
        [FromBody] ChangeCampaignExpenseStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeExpenseStatusAsync(campaignExpenseID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Campaign expense status changed successfully."));
    }

    [HttpPost("{campaignID:int}/attributions")]
    [HasPermission(PermissionCodes.CampaignAttributionManage)]
    public async Task<ActionResult<ApiResponse<int>>> AddAttribution(
        int campaignID,
        [FromBody] SaveCampaignAttributionRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.AddAttributionAsync(campaignID, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Campaign attribution added successfully."));
    }

    [HttpGet("{campaignID:int}/roi")]
    [HasPermission(PermissionCodes.CampaignsView)]
    public async Task<ActionResult<ApiResponse<CampaignRoiDto>>> GetRoi(
            int campaignID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetRoiAsync(campaignID, cancellationToken);
        return Ok(ApiResponse<CampaignRoiDto>.Success(result, "Campaign ROI retrieved successfully."));
    }
}
