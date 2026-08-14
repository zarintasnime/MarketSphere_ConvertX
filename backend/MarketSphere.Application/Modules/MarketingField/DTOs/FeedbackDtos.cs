using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.MarketingField.DTOs;

public sealed record FeedbackListDto(int FeedbackID, int? ClientID, int? LeadID, int? CampaignID, int? VisitID, int? SubmittedByEmployeeID, FeedbackType FeedbackType, int? Rating, DateTime SubmittedAt, bool IsFollowUpRequired);
public sealed record FeedbackDetailsDto(int FeedbackID, int? ClientID, int? LeadID, int? CampaignID, int? VisitID, int? SubmittedByEmployeeID, FeedbackType FeedbackType, int? Rating, string? Comments, DateTime SubmittedAt, bool IsFollowUpRequired);
public sealed class SaveFeedbackRequestDto { public int? ClientID { get; init; } public int? LeadID { get; init; } public int? CampaignID { get; init; } public int? VisitID { get; init; } public int? SubmittedByEmployeeID { get; init; } public FeedbackType FeedbackType { get; init; } public int? Rating { get; init; } public string? Comments { get; init; } public DateTime? SubmittedAt { get; init; } public bool IsFollowUpRequired { get; init; } }
