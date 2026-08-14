using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.CRM.DTOs;

namespace MarketSphere.Application.Modules.CRM.Interfaces;
public interface IClientService
{
    Task<PagedResult<ClientListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<ClientDetailsDto> GetByIdAsync(int clientID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveClientRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int clientID, SaveClientRequestDto request, CancellationToken cancellationToken = default);
    Task<int> AddContactAsync(int clientID, SaveClientContactRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateContactAsync(int clientContactID, SaveClientContactRequestDto request, CancellationToken cancellationToken = default);
    Task SetCreditProfileAsync(int clientID, SaveClientCreditProfileRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeLifecycleAsync(int clientID, ChangeClientLifecycleRequestDto request, CancellationToken cancellationToken = default);
    Task<int> CreateSegmentAsync(SaveClientSegmentRequestDto request, CancellationToken cancellationToken = default);
    Task<int> AssignSegmentAsync(int clientID, AssignClientSegmentRequestDto request, CancellationToken cancellationToken = default);
    Task EndSegmentAssignmentAsync(int clientSegmentAssignmentID, DateTime effectiveTo, CancellationToken cancellationToken = default);
}
