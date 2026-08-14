namespace MarketSphere.Domain.Enums;

public enum ClientType { Outlet = 1, Dealer = 2, Distributor = 3, ModernTrade = 4, BusinessPartner = 5 }
public enum SalesChannel { GeneralTrade = 1, ModernTrade = 2, BusinessPartner = 3, Institutional = 4, Online = 5 }
public enum ClientLifecycleStatus { Prospect = 1, Active = 2, Inactive = 3, Churned = 4, ReactivationInProgress = 5 }
public enum ClientRiskStatus { Normal = 1, Watch = 2, HighRisk = 3, Blocked = 4 }
public enum LeadSource { Manual = 1, Referral = 2, Campaign = 3, FieldVisit = 4, Website = 5, Phone = 6, Other = 7 }
public enum LeadTemperature { Cold = 1, Warm = 2, Hot = 3 }
public enum LeadStatus { New = 1, Contacted = 2, Qualified = 3, Interested = 4, SampleGiven = 5, Negotiation = 6, Converted = 7, Lost = 8 }
public enum LeadScoreConditionType { Source = 1, EstimatedValue = 2, HasPhone = 3, HasEmail = 4, ProductInterest = 5, Region = 6, NextFollowUp = 7 }
public enum ComparisonOperator { Equals = 1, NotEquals = 2, GreaterThan = 3, GreaterThanOrEqual = 4, LessThan = 5, LessThanOrEqual = 6, Contains = 7, IsTrue = 8, IsFalse = 9 }
public enum DuplicateReviewStatus { Open = 1, UnderReview = 2, Resolved = 3, Dismissed = 4 }
public enum DuplicateResolutionType { NotDuplicate = 1, Linked = 2, Merged = 3, KeptSeparate = 4 }
public enum CrmActivityType { Call = 1, Meeting = 2, Email = 3, Note = 4, Visit = 5, Demo = 6 }
public enum CrmActivityStatus { Planned = 1, InProgress = 2, Completed = 3, Cancelled = 4, NoShow = 5 }
public enum ParticipantRole { Organizer = 1, Attendee = 2, DecisionMaker = 3, Influencer = 4, Observer = 5 }
public enum AttendanceStatus { Invited = 1, Accepted = 2, Declined = 3, Attended = 4, Absent = 5 }
public enum TaskPriority { Low = 1, Normal = 2, High = 3, Urgent = 4 }
public enum CrmTaskStatus { Open = 1, InProgress = 2, Completed = 3, Cancelled = 4 }
public enum OpportunityStage { Qualified = 1, RequirementAnalysis = 2, Proposal = 3, Negotiation = 4, Commit = 5, Won = 6, Lost = 7, OnHold = 8 }
public enum QuotationStatus { Draft = 1, Submitted = 2, Reviewed = 3, Accepted = 4, Rejected = 5, Expired = 6, Converted = 7 }
public enum ClientSegmentType { Manual = 1, Lifecycle = 2, Channel = 3, Value = 4, Risk = 5 }
public enum ComplaintCategory { ProductQuality = 1, Delivery = 2, Billing = 3, Service = 4, Pricing = 5, Other = 6 }
public enum ComplaintPriority { Low = 1, Normal = 2, High = 3, Critical = 4 }
public enum ComplaintStatus { Open = 1, Assigned = 2, InProgress = 3, WaitingForCustomer = 4, Resolved = 5, Closed = 6, Rejected = 7 }
public enum ReactivationCaseStatus { Open = 1, Contacted = 2, InProgress = 3, Successful = 4, Unsuccessful = 5, Closed = 6 }
public enum ReactivationResult { Reordered = 1, Interested = 2, NotInterested = 3, Unreachable = 4, CompetitorRetained = 5, BusinessClosed = 6 }
