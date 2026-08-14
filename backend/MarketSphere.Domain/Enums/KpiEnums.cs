namespace MarketSphere.Domain.Enums;

public enum TargetType
{
    SalesAmount = 1,
    SalesQuantity = 2,
    CollectionAmount = 3,
    VisitCount = 4,
    NewClientCount = 5,
    VerifiedSellOutAmount = 6,
    CampaignAchievement = 7,
    Custom = 8
}

public enum EmployeeTargetStatus
{
    Draft = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4
}

public enum RewardType
{
    Incentive = 1,
    Commission = 2,
    Bonus = 3
}

public enum RewardCalculationType
{
    FixedAmount = 1,
    Percentage = 2,
    AchievementSlab = 3
}

public enum RewardCalculationStatus
{
    Draft = 1,
    Calculated = 2,
    Submitted = 3,
    Approved = 4,
    Rejected = 5,
    Paid = 6
}
