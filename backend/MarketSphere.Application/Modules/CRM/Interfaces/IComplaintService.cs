using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.CRM.DTOs;
namespace MarketSphere.Application.Modules.CRM.Interfaces;
public interface IComplaintService
{
    Task<PagedResult<ComplaintListDto>> GetPagedAsync(PagedRequest request, bool slaBreachedOnly, CancellationToken cancellationToken = default);
    Task<ComplaintDetailsDto> GetByIdAsync(int complaintID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveComplaintRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int complaintID, SaveComplaintRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int complaintID, ChangeComplaintStatusRequestDto request, CancellationToken cancellationToken = default);
}
