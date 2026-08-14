namespace MarketSphere.Domain.Common;

public interface IHasStatus<TStatus> where TStatus : struct, Enum
{
    TStatus Status { get; set; }
}
