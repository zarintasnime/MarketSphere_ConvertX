using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.CRM.DTOs;
namespace MarketSphere.Application.Modules.CRM.Interfaces;
public interface IQuotationService
{
    Task<PagedResult<QuotationListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<QuotationDetailsDto> GetByIdAsync(int quotationID, CancellationToken cancellationToken = default);
    Task<int> CreateDraftAsync(SaveQuotationRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateDraftAsync(int quotationID, SaveQuotationRequestDto request, CancellationToken cancellationToken = default);
    Task<int> CreateNewVersionAsync(int quotationID, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int quotationID, ChangeQuotationStatusRequestDto request, CancellationToken cancellationToken = default);
}
