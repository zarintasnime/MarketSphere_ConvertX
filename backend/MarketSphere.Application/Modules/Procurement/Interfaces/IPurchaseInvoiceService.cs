using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Procurement.DTOs;
namespace MarketSphere.Application.Modules.Procurement.Interfaces;
public interface IPurchaseInvoiceService
{
    Task<PagedResult<PurchaseInvoiceDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SavePurchaseInvoiceRequestDto request, CancellationToken cancellationToken = default);
    Task ConfirmAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreatePaymentAsync(CreateSupplierPaymentRequestDto request, CancellationToken cancellationToken = default);
    Task ChangePaymentStatusAsync(int supplierPaymentID, ChangeSupplierPaymentStatusRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SupplierPaymentDto>> GetPaymentsAsync(int purchaseInvoiceID, CancellationToken cancellationToken = default);
}
