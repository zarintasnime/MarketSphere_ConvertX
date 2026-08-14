using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _05_ProcurementInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    BatchID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ManufacturingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BestBeforeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.BatchID);
                    table.CheckConstraint("CK_Batches_CostPrice", "[CostPrice] >= 0");
                    table.CheckConstraint("CK_Batches_Dates", "[ExpiryDate] IS NULL OR [ManufacturingDate] IS NULL OR [ExpiryDate] >= [ManufacturingDate]");
                    table.ForeignKey(
                        name: "FK_Batches_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequisitions",
                columns: table => new
                {
                    PurchaseRequisitionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequisitionNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchID = table.Column<int>(type: "int", nullable: false),
                    RequestedByEmployeeID = table.Column<int>(type: "int", nullable: false),
                    RequiredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequisitions", x => x.PurchaseRequisitionID);
                    table.ForeignKey(
                        name: "FK_PurchaseRequisitions_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequisitions_Employees_RequestedByEmployeeID",
                        column: x => x.RequestedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaymentTermsDays = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierID);
                    table.CheckConstraint("CK_Suppliers_PaymentTerms", "[PaymentTermsDays] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    WarehouseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchID = table.Column<int>(type: "int", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    WarehouseType = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.WarehouseID);
                    table.ForeignKey(
                        name: "FK_Warehouses_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequisitionItems",
                columns: table => new
                {
                    PurchaseRequisitionItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequisitionID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    EstimatedUnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequisitionItems", x => x.PurchaseRequisitionItemID);
                    table.CheckConstraint("CK_PurchaseRequisitionItems_Cost", "[EstimatedUnitCost] IS NULL OR [EstimatedUnitCost] >= 0");
                    table.CheckConstraint("CK_PurchaseRequisitionItems_Quantity", "[RequestedQuantity] > 0");
                    table.ForeignKey(
                        name: "FK_PurchaseRequisitionItems_PurchaseRequisitions_PurchaseRequisitionID",
                        column: x => x.PurchaseRequisitionID,
                        principalTable: "PurchaseRequisitions",
                        principalColumn: "PurchaseRequisitionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequisitionItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    PurchaseOrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    PurchaseRequisitionID = table.Column<int>(type: "int", nullable: true),
                    BranchID = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.PurchaseOrderID);
                    table.CheckConstraint("CK_PurchaseOrders_Amounts", "[GrossAmount] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [NetAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_PurchaseRequisitions_PurchaseRequisitionID",
                        column: x => x.PurchaseRequisitionID,
                        principalTable: "PurchaseRequisitions",
                        principalColumn: "PurchaseRequisitionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierProducts",
                columns: table => new
                {
                    SupplierProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    SupplierSKUCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    LastPurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MinimumOrderQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: true),
                    IsPreferredSupplier = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProducts", x => x.SupplierProductID);
                    table.CheckConstraint("CK_SupplierProducts_LastPrice", "[LastPurchasePrice] IS NULL OR [LastPurchasePrice] >= 0");
                    table.CheckConstraint("CK_SupplierProducts_LeadTime", "[LeadTimeDays] IS NULL OR [LeadTimeDays] >= 0");
                    table.CheckConstraint("CK_SupplierProducts_MinQty", "[MinimumOrderQuantity] IS NULL OR [MinimumOrderQuantity] > 0");
                    table.ForeignKey(
                        name: "FK_SupplierProducts_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProducts_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustments",
                columns: table => new
                {
                    StockAdjustmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockAdjustmentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    AdjustmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PerformedByEmployeeID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustments", x => x.StockAdjustmentID);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Employees_PerformedByEmployeeID",
                        column: x => x.PerformedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockBalances",
                columns: table => new
                {
                    StockBalanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    BatchID = table.Column<int>(type: "int", nullable: true),
                    OnHandQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    QuarantineQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    DamagedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockBalances", x => x.StockBalanceID);
                    table.CheckConstraint("CK_StockBalances_Allocations", "([ReservedQuantity] + [QuarantineQuantity] + [DamagedQuantity]) <= [OnHandQuantity]");
                    table.CheckConstraint("CK_StockBalances_NonNegative", "[OnHandQuantity] >= 0 AND [ReservedQuantity] >= 0 AND [QuarantineQuantity] >= 0 AND [DamagedQuantity] >= 0");
                    table.ForeignKey(
                        name: "FK_StockBalances_Batches_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batches",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockBalances_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockBalances_Warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    StockMovementID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    BatchID = table.Column<int>(type: "int", nullable: true),
                    MovementType = table.Column<int>(type: "int", nullable: false),
                    QuantityIn = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    QuantityOut = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ReferenceID = table.Column<int>(type: "int", nullable: false),
                    MovementAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedByUserID = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.StockMovementID);
                    table.CheckConstraint("CK_StockMovements_Balance", "[BalanceAfter] >= 0");
                    table.CheckConstraint("CK_StockMovements_OneDirection", "([QuantityIn] > 0 AND [QuantityOut] = 0) OR ([QuantityOut] > 0 AND [QuantityIn] = 0)");
                    table.ForeignKey(
                        name: "FK_StockMovements_Batches_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batches",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Users_PerformedByUserID",
                        column: x => x.PerformedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockReservations",
                columns: table => new
                {
                    StockReservationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemID = table.Column<int>(type: "int", nullable: false),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    BatchID = table.Column<int>(type: "int", nullable: true),
                    ReservedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReservationStatus = table.Column<int>(type: "int", nullable: false),
                    ReservedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockReservations", x => x.StockReservationID);
                    table.CheckConstraint("CK_StockReservations_Quantity", "[ReservedQuantity] > 0");
                    table.ForeignKey(
                        name: "FK_StockReservations_Batches_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batches",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockReservations_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockReservations_Warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTransfers",
                columns: table => new
                {
                    StockTransferID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockTransferNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FromWarehouseID = table.Column<int>(type: "int", nullable: false),
                    ToWarehouseID = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DispatchedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovalRequestID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransfers", x => x.StockTransferID);
                    table.CheckConstraint("CK_StockTransfers_DifferentWarehouses", "[FromWarehouseID] <> [ToWarehouseID]");
                    table.ForeignKey(
                        name: "FK_StockTransfers_Warehouses_FromWarehouseID",
                        column: x => x.FromWarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransfers_Warehouses_ToWarehouseID",
                        column: x => x.ToWarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceipts",
                columns: table => new
                {
                    GoodsReceiptID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiptNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseOrderID = table.Column<int>(type: "int", nullable: false),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedByEmployeeID = table.Column<int>(type: "int", nullable: false),
                    SupplierChallanNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QualityCheckStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceipts", x => x.GoodsReceiptID);
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_Employees_ReceivedByEmployeeID",
                        column: x => x.ReceivedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_PurchaseOrders_PurchaseOrderID",
                        column: x => x.PurchaseOrderID,
                        principalTable: "PurchaseOrders",
                        principalColumn: "PurchaseOrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_Warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    PurchaseOrderItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    OrderedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.PurchaseOrderItemID);
                    table.CheckConstraint("CK_PurchaseOrderItems_Amounts", "[UnitCost] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [LineTotal] >= 0");
                    table.CheckConstraint("CK_PurchaseOrderItems_Quantities", "[OrderedQuantity] > 0 AND [ReceivedQuantity] >= 0 AND [ReceivedQuantity] <= [OrderedQuantity]");
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderID",
                        column: x => x.PurchaseOrderID,
                        principalTable: "PurchaseOrders",
                        principalColumn: "PurchaseOrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustmentItems",
                columns: table => new
                {
                    StockAdjustmentItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockAdjustmentID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    BatchID = table.Column<int>(type: "int", nullable: true),
                    AdjustmentQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StockMovementID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustmentItems", x => x.StockAdjustmentItemID);
                    table.CheckConstraint("CK_StockAdjustmentItems_Quantity", "[AdjustmentQuantity] <> 0");
                    table.CheckConstraint("CK_StockAdjustmentItems_UnitCost", "[UnitCost] IS NULL OR [UnitCost] >= 0");
                    table.ForeignKey(
                        name: "FK_StockAdjustmentItems_Batches_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batches",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustmentItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustmentItems_StockAdjustments_StockAdjustmentID",
                        column: x => x.StockAdjustmentID,
                        principalTable: "StockAdjustments",
                        principalColumn: "StockAdjustmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockAdjustmentItems_StockMovements_StockMovementID",
                        column: x => x.StockMovementID,
                        principalTable: "StockMovements",
                        principalColumn: "StockMovementID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTransferItems",
                columns: table => new
                {
                    StockTransferItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockTransferID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    BatchID = table.Column<int>(type: "int", nullable: true),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    DispatchedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransferItems", x => x.StockTransferItemID);
                    table.CheckConstraint("CK_StockTransferItems_Quantities", "[RequestedQuantity] > 0 AND [DispatchedQuantity] >= 0 AND [ReceivedQuantity] >= 0 AND [ReceivedQuantity] <= [DispatchedQuantity] AND [DispatchedQuantity] <= [RequestedQuantity]");
                    table.ForeignKey(
                        name: "FK_StockTransferItems_Batches_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batches",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransferItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransferItems_StockTransfers_StockTransferID",
                        column: x => x.StockTransferID,
                        principalTable: "StockTransfers",
                        principalColumn: "StockTransferID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseInvoices",
                columns: table => new
                {
                    PurchaseInvoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseInvoiceNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderID = table.Column<int>(type: "int", nullable: true),
                    GoodsReceiptID = table.Column<int>(type: "int", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DueAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoices", x => x.PurchaseInvoiceID);
                    table.CheckConstraint("CK_PurchaseInvoices_Amounts", "[GrossAmount] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [TotalAmount] >= 0 AND [PaidAmount] >= 0 AND [DueAmount] >= 0 AND ([PaidAmount] + [DueAmount]) = [TotalAmount]");
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_GoodsReceipts_GoodsReceiptID",
                        column: x => x.GoodsReceiptID,
                        principalTable: "GoodsReceipts",
                        principalColumn: "GoodsReceiptID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_PurchaseOrders_PurchaseOrderID",
                        column: x => x.PurchaseOrderID,
                        principalTable: "PurchaseOrders",
                        principalColumn: "PurchaseOrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierReturns",
                columns: table => new
                {
                    SupplierReturnID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierReturnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    GoodsReceiptID = table.Column<int>(type: "int", nullable: true),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierReturns", x => x.SupplierReturnID);
                    table.ForeignKey(
                        name: "FK_SupplierReturns_GoodsReceipts_GoodsReceiptID",
                        column: x => x.GoodsReceiptID,
                        principalTable: "GoodsReceipts",
                        principalColumn: "GoodsReceiptID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierReturns_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierReturns_Warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptItems",
                columns: table => new
                {
                    GoodsReceiptItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiptID = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderItemID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    AcceptedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    RejectedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ManufacturingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BatchID = table.Column<int>(type: "int", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptItems", x => x.GoodsReceiptItemID);
                    table.CheckConstraint("CK_GoodsReceiptItems_Quantities", "[AcceptedQuantity] >= 0 AND [RejectedQuantity] >= 0 AND ([AcceptedQuantity] + [RejectedQuantity]) > 0");
                    table.CheckConstraint("CK_GoodsReceiptItems_RejectionReason", "[RejectedQuantity] = 0 OR [RejectionReason] IS NOT NULL");
                    table.CheckConstraint("CK_GoodsReceiptItems_UnitCost", "[UnitCost] >= 0");
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItems_Batches_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batches",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItems_GoodsReceipts_GoodsReceiptID",
                        column: x => x.GoodsReceiptID,
                        principalTable: "GoodsReceipts",
                        principalColumn: "GoodsReceiptID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItems_PurchaseOrderItems_PurchaseOrderItemID",
                        column: x => x.PurchaseOrderItemID,
                        principalTable: "PurchaseOrderItems",
                        principalColumn: "PurchaseOrderItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierPayments",
                columns: table => new
                {
                    SupplierPaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    PurchaseInvoiceID = table.Column<int>(type: "int", nullable: false),
                    PaymentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPayments", x => x.SupplierPaymentID);
                    table.CheckConstraint("CK_SupplierPayments_Amount", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_SupplierPayments_PurchaseInvoices_PurchaseInvoiceID",
                        column: x => x.PurchaseInvoiceID,
                        principalTable: "PurchaseInvoices",
                        principalColumn: "PurchaseInvoiceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPayments_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierReturnItems",
                columns: table => new
                {
                    SupplierReturnItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierReturnID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    BatchID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    StockMovementID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierReturnItems", x => x.SupplierReturnItemID);
                    table.CheckConstraint("CK_SupplierReturnItems_Quantity", "[Quantity] > 0");
                    table.CheckConstraint("CK_SupplierReturnItems_UnitCost", "[UnitCost] >= 0");
                    table.ForeignKey(
                        name: "FK_SupplierReturnItems_Batches_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batches",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierReturnItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierReturnItems_StockMovements_StockMovementID",
                        column: x => x.StockMovementID,
                        principalTable: "StockMovements",
                        principalColumn: "StockMovementID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierReturnItems_SupplierReturns_SupplierReturnID",
                        column: x => x.SupplierReturnID,
                        principalTable: "SupplierReturns",
                        principalColumn: "SupplierReturnID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batches_ExpiryDate_Status",
                table: "Batches",
                columns: new[] { "ExpiryDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Batches_SKUID_BatchNo",
                table: "Batches",
                columns: new[] { "SKUID", "BatchNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_BatchID",
                table: "GoodsReceiptItems",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_GoodsReceiptID_PurchaseOrderItemID",
                table: "GoodsReceiptItems",
                columns: new[] { "GoodsReceiptID", "PurchaseOrderItemID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_PurchaseOrderItemID",
                table: "GoodsReceiptItems",
                column: "PurchaseOrderItemID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_SKUID",
                table: "GoodsReceiptItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_GoodsReceiptNo",
                table: "GoodsReceipts",
                column: "GoodsReceiptNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_PurchaseOrderID_Status_ReceivedDate",
                table: "GoodsReceipts",
                columns: new[] { "PurchaseOrderID", "Status", "ReceivedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_ReceivedByEmployeeID",
                table: "GoodsReceipts",
                column: "ReceivedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_WarehouseID",
                table: "GoodsReceipts",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_GoodsReceiptID",
                table: "PurchaseInvoices",
                column: "GoodsReceiptID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_PurchaseOrderID",
                table: "PurchaseInvoices",
                column: "PurchaseOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_SupplierID_PurchaseInvoiceNo",
                table: "PurchaseInvoices",
                columns: new[] { "SupplierID", "PurchaseInvoiceNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_PurchaseOrderID_SKUID",
                table: "PurchaseOrderItems",
                columns: new[] { "PurchaseOrderID", "SKUID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_SKUID",
                table: "PurchaseOrderItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_BranchID",
                table: "PurchaseOrders",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PurchaseOrderNo",
                table: "PurchaseOrders",
                column: "PurchaseOrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PurchaseRequisitionID",
                table: "PurchaseOrders",
                column: "PurchaseRequisitionID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SupplierID_Status_OrderDate",
                table: "PurchaseOrders",
                columns: new[] { "SupplierID", "Status", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitionItems_PurchaseRequisitionID_SKUID",
                table: "PurchaseRequisitionItems",
                columns: new[] { "PurchaseRequisitionID", "SKUID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitionItems_SKUID",
                table: "PurchaseRequisitionItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitions_BranchID_Status_RequiredDate",
                table: "PurchaseRequisitions",
                columns: new[] { "BranchID", "Status", "RequiredDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitions_PurchaseRequisitionNo",
                table: "PurchaseRequisitions",
                column: "PurchaseRequisitionNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitions_RequestedByEmployeeID",
                table: "PurchaseRequisitions",
                column: "RequestedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItems_BatchID",
                table: "StockAdjustmentItems",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItems_SKUID",
                table: "StockAdjustmentItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItems_StockAdjustmentID_SKUID_BatchID",
                table: "StockAdjustmentItems",
                columns: new[] { "StockAdjustmentID", "SKUID", "BatchID" },
                unique: true,
                filter: "[BatchID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItems_StockMovementID",
                table: "StockAdjustmentItems",
                column: "StockMovementID");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_PerformedByEmployeeID",
                table: "StockAdjustments",
                column: "PerformedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_StockAdjustmentNo",
                table: "StockAdjustments",
                column: "StockAdjustmentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_WarehouseID_Status_AdjustmentDate",
                table: "StockAdjustments",
                columns: new[] { "WarehouseID", "Status", "AdjustmentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StockBalances_BatchID",
                table: "StockBalances",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalances_SKUID",
                table: "StockBalances",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalances_WarehouseID_SKUID_BatchID",
                table: "StockBalances",
                columns: new[] { "WarehouseID", "SKUID", "BatchID" },
                unique: true,
                filter: "[BatchID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_BatchID",
                table: "StockMovements",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_PerformedByUserID",
                table: "StockMovements",
                column: "PerformedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ReferenceType_ReferenceID",
                table: "StockMovements",
                columns: new[] { "ReferenceType", "ReferenceID" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_SKUID",
                table: "StockMovements",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_WarehouseID_SKUID_BatchID_MovementAt",
                table: "StockMovements",
                columns: new[] { "WarehouseID", "SKUID", "BatchID", "MovementAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockReservations_BatchID",
                table: "StockReservations",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_StockReservations_OrderItemID_ReservationStatus",
                table: "StockReservations",
                columns: new[] { "OrderItemID", "ReservationStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_StockReservations_SKUID",
                table: "StockReservations",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_StockReservations_WarehouseID_SKUID_BatchID_ReservationStatus",
                table: "StockReservations",
                columns: new[] { "WarehouseID", "SKUID", "BatchID", "ReservationStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_BatchID",
                table: "StockTransferItems",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_SKUID",
                table: "StockTransferItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_StockTransferID_SKUID_BatchID",
                table: "StockTransferItems",
                columns: new[] { "StockTransferID", "SKUID", "BatchID" },
                unique: true,
                filter: "[BatchID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_FromWarehouseID_ToWarehouseID_Status_RequestedAt",
                table: "StockTransfers",
                columns: new[] { "FromWarehouseID", "ToWarehouseID", "Status", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_StockTransferNo",
                table: "StockTransfers",
                column: "StockTransferNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ToWarehouseID",
                table: "StockTransfers",
                column: "ToWarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_PaymentNo",
                table: "SupplierPayments",
                column: "PaymentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_PurchaseInvoiceID",
                table: "SupplierPayments",
                column: "PurchaseInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_SupplierID",
                table: "SupplierPayments",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProducts_SKUID",
                table: "SupplierProducts",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProducts_SupplierID_SKUID",
                table: "SupplierProducts",
                columns: new[] { "SupplierID", "SKUID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReturnItems_BatchID",
                table: "SupplierReturnItems",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReturnItems_SKUID",
                table: "SupplierReturnItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReturnItems_StockMovementID",
                table: "SupplierReturnItems",
                column: "StockMovementID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReturnItems_SupplierReturnID_SKUID_BatchID",
                table: "SupplierReturnItems",
                columns: new[] { "SupplierReturnID", "SKUID", "BatchID" },
                unique: true,
                filter: "[BatchID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReturns_GoodsReceiptID",
                table: "SupplierReturns",
                column: "GoodsReceiptID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReturns_SupplierID_Status_ReturnDate",
                table: "SupplierReturns",
                columns: new[] { "SupplierID", "Status", "ReturnDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReturns_SupplierReturnNo",
                table: "SupplierReturns",
                column: "SupplierReturnNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReturns_WarehouseID",
                table: "SupplierReturns",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierCode",
                table: "Suppliers",
                column: "SupplierCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierName_Status",
                table: "Suppliers",
                columns: new[] { "SupplierName", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_BranchID_WarehouseCode",
                table: "Warehouses",
                columns: new[] { "BranchID", "WarehouseCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsReceiptItems");

            migrationBuilder.DropTable(
                name: "PurchaseRequisitionItems");

            migrationBuilder.DropTable(
                name: "StockAdjustmentItems");

            migrationBuilder.DropTable(
                name: "StockBalances");

            migrationBuilder.DropTable(
                name: "StockReservations");

            migrationBuilder.DropTable(
                name: "StockTransferItems");

            migrationBuilder.DropTable(
                name: "SupplierPayments");

            migrationBuilder.DropTable(
                name: "SupplierProducts");

            migrationBuilder.DropTable(
                name: "SupplierReturnItems");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "StockAdjustments");

            migrationBuilder.DropTable(
                name: "StockTransfers");

            migrationBuilder.DropTable(
                name: "PurchaseInvoices");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "SupplierReturns");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "GoodsReceipts");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "PurchaseRequisitions");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
