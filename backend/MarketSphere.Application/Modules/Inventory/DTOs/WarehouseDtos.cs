using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Inventory.DTOs;

public sealed record WarehouseDto(int WarehouseID, int BranchID, string BranchName, string WarehouseCode, string WarehouseName, WarehouseType WarehouseType, string? Address, bool IsActive);
public sealed record SaveWarehouseRequestDto(int BranchID, string WarehouseCode, string WarehouseName, WarehouseType WarehouseType, string? Address);
public sealed record ChangeWarehouseStatusRequestDto(bool IsActive);
