using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
namespace MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
public interface IInvoiceService { Task<PagedResult<InvoiceListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default); Task<InvoiceDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default); Task<int> CreateFromOrderAsync(CreateInvoiceRequestDto request, CancellationToken cancellationToken = default); Task ChangeStatusAsync(int id, ChangeInvoiceStatusRequestDto request, CancellationToken cancellationToken = default); }
