using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
namespace MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
public interface IPaymentService { Task<PagedResult<PaymentListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default); Task<PaymentDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default); Task<int> CreateAsync(CreatePaymentRequestDto request, CancellationToken cancellationToken = default); Task ConfirmAsync(int id, ConfirmPaymentRequestDto request, CancellationToken cancellationToken = default); Task ReverseAllocationAsync(ReversePaymentAllocationRequestDto request, CancellationToken cancellationToken = default); }
