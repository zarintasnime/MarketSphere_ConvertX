namespace MarketSphere.Application.Common.Interfaces;

public interface INumberSequenceService
{
    Task<string> NextAsync(
        string documentType,
        int? branchID,
        CancellationToken cancellationToken = default);
}
