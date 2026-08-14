namespace MarketSphere.Domain.Enums;

public enum CampaignStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4,
    Active = 5,
    Paused = 6,
    Completed = 7,
    Evaluated = 8
}

public enum CampaignTargetType
{
    Region = 1,
    Area = 2,
    ClientSegment = 3,
    Client = 4,
    ProductCategory = 5,
    SKU = 6
}

public enum CampaignOfferType
{
    PercentageDiscount = 1,
    FixedDiscount = 2,
    FreeItem = 3,
    Bundle = 4,
    Cashback = 5,
    Other = 6
}

public enum CampaignExpenseStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4,
    Posted = 5
}

public enum CampaignAttributionType
{
    FirstTouch = 1,
    LastTouch = 2,
    Linear = 3,
    Manual = 4
}

public enum VisitType
{
    Sales = 1,
    Marketing = 2,
    Merchandising = 3,
    Sampling = 4,
    Collection = 5,
    Support = 6
}

public enum VisitStatus
{
    CheckedIn = 1,
    Completed = 2,
    Cancelled = 3
}

public enum SamplingOutcome
{
    Positive = 1,
    Neutral = 2,
    Negative = 3,
    FollowUpRequired = 4
}

public enum FeedbackType
{
    Product = 1,
    Service = 2,
    Campaign = 3,
    Sampling = 4,
    Complaint = 5,
    Other = 6
}

public enum MarketObservationType
{
    Availability = 1,
    Planogram = 2,
    Display = 3,
    Price = 4,
    Competitor = 5,
    Promotion = 6
}

public enum AvailabilityStatus
{
    Available = 1,
    LowStock = 2,
    OutOfStock = 3,
    NotListed = 4
}

public enum VerificationStatus
{
    Pending = 1,
    Verified = 2,
    Rejected = 3
}
