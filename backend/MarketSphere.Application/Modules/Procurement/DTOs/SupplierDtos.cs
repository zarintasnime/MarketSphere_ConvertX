using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Procurement.DTOs;

public sealed record SupplierListDto(int SupplierID, string SupplierCode, string SupplierName, string? Phone, int PaymentTermsDays, SupplierStatus Status);
public sealed record SupplierProductDto(int SupplierProductID, int SKUID, string SKUCode, string SKUName, string? SupplierSKUCode, decimal? LastPurchasePrice, decimal? MinimumOrderQuantity, int? LeadTimeDays, bool IsPreferredSupplier, bool IsActive);
public sealed record SupplierDetailsDto(int SupplierID, string SupplierCode, string SupplierName, string? ContactPerson, string? Phone, string? Email, string? Address, int PaymentTermsDays, SupplierStatus Status, IReadOnlyCollection<SupplierProductDto> Products);
public sealed record SaveSupplierRequestDto(string SupplierCode, string SupplierName, string? ContactPerson, string? Phone, string? Email, string? Address, int PaymentTermsDays);
public sealed record SaveSupplierProductRequestDto(int SKUID, string? SupplierSKUCode, decimal? LastPurchasePrice, decimal? MinimumOrderQuantity, int? LeadTimeDays, bool IsPreferredSupplier, bool IsActive);
public sealed record ChangeSupplierStatusRequestDto(SupplierStatus Status);
