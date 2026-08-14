using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.MarketingField.DTOs;

namespace MarketSphere.Application.Modules.MarketingField.Interfaces;

public interface IVisitService
{
    Task<PagedResult<VisitListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<VisitDetailsDto> GetByIdAsync(int visitID, CancellationToken cancellationToken = default);
    Task<int> CheckInAsync(CheckInVisitRequestDto request, CancellationToken cancellationToken = default);
    Task CheckOutAsync(int visitID, CheckOutVisitRequestDto request, CancellationToken cancellationToken = default);
    Task CancelAsync(int visitID, CancelVisitRequestDto request, CancellationToken cancellationToken = default);
}
