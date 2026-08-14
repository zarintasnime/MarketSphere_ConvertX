namespace MarketSphere.Domain.Enums;

public enum BranchType
{
    HeadOffice = 1,
    RegionalOffice = 2,
    Depot = 3
}

public enum UserStatus
{
    Invited = 1,
    Active = 2,
    Locked = 3,
    Disabled = 4
}

public enum AccountTokenType
{
    Activation = 1,
    PasswordReset = 2
}

public enum EmployeeStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Terminated = 4
}

public enum GeographyScopeType
{
    Region = 1,
    Area = 2,
    Territory = 3
}

public enum AssignmentStatus
{
    Active = 1,
    Inactive = 2
}

public enum VisitFrequency
{
    Daily = 1,
    Weekly = 2,
    BiWeekly = 3,
    Monthly = 4,
    Custom = 5
}
