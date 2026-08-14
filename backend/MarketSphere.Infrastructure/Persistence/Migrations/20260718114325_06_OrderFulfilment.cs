using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _06_OrderFulfilment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Complaints_Status_SLADueAt_AssignedEmployeeID",
                table: "Complaints");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Complaints_Satisfaction",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_CampaignAttributions_CampaignID_AttributionType",
                table: "CampaignAttributions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CampaignAttributions_Amount",
                table: "CampaignAttributions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CampaignAttributions_Stage",
                table: "CampaignAttributions");

            migrationBuilder.AlterColumn<string>(
                name: "ChurnReason",
                table: "ReactivationCases",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResolutionNote",
                table: "Complaints",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "Complaints",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ModernTradePurchaseOrders",
                columns: table => new
                {
                    ModernTradePurchaseOrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PODate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedByEmployeeID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    CompletenessStatus = table.Column<int>(type: "int", nullable: false),
                    VerificationNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VerifiedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DuplicateHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RequestedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModernTradePurchaseOrders", x => x.ModernTradePurchaseOrderID);
                    table.CheckConstraint("CK_ModernTradePurchaseOrders_Dates", "[ReceivedDate] >= [PODate]");
                    table.ForeignKey(
                        name: "FK_ModernTradePurchaseOrders_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModernTradePurchaseOrders_Employees_UploadedByEmployeeID",
                        column: x => x.UploadedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModernTradePurchaseOrders_Employees_VerifiedByEmployeeID",
                        column: x => x.VerifiedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProofFileAttachmentID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReceivedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentID);
                    table.CheckConstraint("CK_Payments_Amount", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_Payments_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Users_ReceivedByUserID",
                        column: x => x.ReceivedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModernTradePurchaseOrderItems",
                columns: table => new
                {
                    ModernTradePurchaseOrderItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModernTradePurchaseOrderID = table.Column<int>(type: "int", nullable: false),
                    ExternalItemCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExternalItemName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SKUID = table.Column<int>(type: "int", nullable: true),
                    MappingStatus = table.Column<int>(type: "int", nullable: false),
                    OrderedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    AgreedUnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModernTradePurchaseOrderItems", x => x.ModernTradePurchaseOrderItemID);
                    table.CheckConstraint("CK_ModernTradePurchaseOrderItems_Quantity", "[OrderedQuantity] > 0 AND ([AgreedUnitPrice] IS NULL OR [AgreedUnitPrice] >= 0) AND ([Discount] IS NULL OR [Discount] >= 0)");
                    table.ForeignKey(
                        name: "FK_ModernTradePurchaseOrderItems_ModernTradePurchaseOrders_ModernTradePurchaseOrderID",
                        column: x => x.ModernTradePurchaseOrderID,
                        principalTable: "ModernTradePurchaseOrders",
                        principalColumn: "ModernTradePurchaseOrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModernTradePurchaseOrderItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: true),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    OrderSource = table.Column<int>(type: "int", nullable: false),
                    CampaignID = table.Column<int>(type: "int", nullable: true),
                    QuotationID = table.Column<int>(type: "int", nullable: true),
                    ModernTradePurchaseOrderID = table.Column<int>(type: "int", nullable: true),
                    PriceListID = table.Column<int>(type: "int", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryAddressSnapshot = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreditCheckStatus = table.Column<int>(type: "int", nullable: false),
                    ApprovalRequestID = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderID);
                    table.CheckConstraint("CK_Orders_Amounts", "[GrossAmount] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [NetAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_Orders_Campaigns_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_ModernTradePurchaseOrders_ModernTradePurchaseOrderID",
                        column: x => x.ModernTradePurchaseOrderID,
                        principalTable: "ModernTradePurchaseOrders",
                        principalColumn: "ModernTradePurchaseOrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_PriceLists_PriceListID",
                        column: x => x.PriceListID,
                        principalTable: "PriceLists",
                        principalColumn: "PriceListID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Quotations_QuotationID",
                        column: x => x.QuotationID,
                        principalTable: "Quotations",
                        principalColumn: "QuotationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DueAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceID);
                    table.CheckConstraint("CK_Invoices_Amounts", "[GrossAmount] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [TotalAmount] >= 0 AND [PaidAmount] >= 0 AND [DueAmount] >= 0");
                    table.CheckConstraint("CK_Invoices_Balance", "[PaidAmount] + [DueAmount] = [TotalAmount]");
                    table.ForeignKey(
                        name: "FK_Invoices_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    OrderedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    FreeQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    DeliveredQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReturnedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    BackorderQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemID);
                    table.CheckConstraint("CK_OrderItems_Amounts", "[UnitPrice] >= 0 AND [DiscountPercent] BETWEEN 0 AND 100 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [LineTotal] >= 0");
                    table.CheckConstraint("CK_OrderItems_Quantities", "[OrderedQuantity] > 0 AND [FreeQuantity] >= 0 AND [ApprovedQuantity] >= 0 AND [DeliveredQuantity] >= 0 AND [ReturnedQuantity] >= 0 AND [BackorderQuantity] >= 0");
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAllocations",
                columns: table => new
                {
                    PaymentAllocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentID = table.Column<int>(type: "int", nullable: false),
                    InvoiceID = table.Column<int>(type: "int", nullable: false),
                    AllocationType = table.Column<int>(type: "int", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReversalOfPaymentAllocationID = table.Column<int>(type: "int", nullable: true),
                    AllocatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AllocatedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAllocations", x => x.PaymentAllocationID);
                    table.CheckConstraint("CK_PaymentAllocations_Amount", "[AllocatedAmount] > 0");
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_PaymentAllocations_ReversalOfPaymentAllocationID",
                        column: x => x.ReversalOfPaymentAllocationID,
                        principalTable: "PaymentAllocations",
                        principalColumn: "PaymentAllocationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_Payments_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payments",
                        principalColumn: "PaymentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_Users_AllocatedByUserID",
                        column: x => x.AllocatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PickLists",
                columns: table => new
                {
                    PickListID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickListNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    InvoiceID = table.Column<int>(type: "int", nullable: true),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    WaveNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleasedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    VerifiedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickLists", x => x.PickListID);
                    table.ForeignKey(
                        name: "FK_PickLists_Employees_ReleasedByEmployeeID",
                        column: x => x.ReleasedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickLists_Employees_VerifiedByEmployeeID",
                        column: x => x.VerifiedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickLists_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickLists_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickLists_Warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppliedOffers",
                columns: table => new
                {
                    AppliedOfferID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationID = table.Column<int>(type: "int", nullable: true),
                    QuotationItemID = table.Column<int>(type: "int", nullable: true),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    OrderItemID = table.Column<int>(type: "int", nullable: true),
                    CampaignOfferID = table.Column<int>(type: "int", nullable: false),
                    BenefitType = table.Column<int>(type: "int", nullable: false),
                    BenefitAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    FreeSKUID = table.Column<int>(type: "int", nullable: true),
                    FreeQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    RuleSnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppliedByUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppliedOffers", x => x.AppliedOfferID);
                    table.CheckConstraint("CK_AppliedOffers_Parent", "(CASE WHEN [QuotationID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [QuotationItemID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [OrderID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [OrderItemID] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.ForeignKey(
                        name: "FK_AppliedOffers_CampaignOffers_CampaignOfferID",
                        column: x => x.CampaignOfferID,
                        principalTable: "CampaignOffers",
                        principalColumn: "CampaignOfferID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppliedOffers_OrderItems_OrderItemID",
                        column: x => x.OrderItemID,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppliedOffers_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppliedOffers_QuotationItems_QuotationItemID",
                        column: x => x.QuotationItemID,
                        principalTable: "QuotationItems",
                        principalColumn: "QuotationItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppliedOffers_Quotations_QuotationID",
                        column: x => x.QuotationID,
                        principalTable: "Quotations",
                        principalColumn: "QuotationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppliedOffers_SKUs_FreeSKUID",
                        column: x => x.FreeSKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppliedOffers_Users_AppliedByUserID",
                        column: x => x.AppliedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                columns: table => new
                {
                    InvoiceItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceID = table.Column<int>(type: "int", nullable: false),
                    OrderItemID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_InvoiceItems", x => x.InvoiceItemID);
                    table.CheckConstraint("CK_InvoiceItems_Values", "[Quantity] > 0 AND [UnitPrice] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [LineTotal] >= 0");
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_OrderItems_OrderItemID",
                        column: x => x.OrderItemID,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    DeliveryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    InvoiceID = table.Column<int>(type: "int", nullable: true),
                    PickListID = table.Column<int>(type: "int", nullable: true),
                    WarehouseID = table.Column<int>(type: "int", nullable: false),
                    PlannedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispatchDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DeliveredByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceiverPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RescheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.DeliveryID);
                    table.ForeignKey(
                        name: "FK_Deliveries_Employees_DeliveredByEmployeeID",
                        column: x => x.DeliveredByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_PickLists_PickListID",
                        column: x => x.PickListID,
                        principalTable: "PickLists",
                        principalColumn: "PickListID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliveries_Warehouses_WarehouseID",
                        column: x => x.WarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PickListItems",
                columns: table => new
                {
                    PickListItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickListID = table.Column<int>(type: "int", nullable: false),
                    OrderItemID = table.Column<int>(type: "int", nullable: false),
                    StockReservationID = table.Column<int>(type: "int", nullable: true),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    BatchID = table.Column<int>(type: "int", nullable: true),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    PickedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ShortQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    PickedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    PickedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickListItems", x => x.PickListItemID);
                    table.CheckConstraint("CK_PickListItems_Quantities", "[RequestedQuantity] > 0 AND [PickedQuantity] >= 0 AND [ShortQuantity] >= 0");
                    table.CheckConstraint("CK_PickListItems_Total", "[PickedQuantity] + [ShortQuantity] <= [RequestedQuantity]");
                    table.ForeignKey(
                        name: "FK_PickListItems_Batches_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batches",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickListItems_Employees_PickedByEmployeeID",
                        column: x => x.PickedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickListItems_OrderItems_OrderItemID",
                        column: x => x.OrderItemID,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickListItems_PickLists_PickListID",
                        column: x => x.PickListID,
                        principalTable: "PickLists",
                        principalColumn: "PickListID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickListItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickListItems_StockReservations_StockReservationID",
                        column: x => x.StockReservationID,
                        principalTable: "StockReservations",
                        principalColumn: "StockReservationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReturnRequests",
                columns: table => new
                {
                    ReturnRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    InvoiceID = table.Column<int>(type: "int", nullable: true),
                    DeliveryID = table.Column<int>(type: "int", nullable: true),
                    ComplaintID = table.Column<int>(type: "int", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReceivedAtWarehouseAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionType = table.Column<int>(type: "int", nullable: true),
                    ReplacementOrderID = table.Column<int>(type: "int", nullable: true),
                    ReplacementDeliveryID = table.Column<int>(type: "int", nullable: true),
                    SupplierReturnID = table.Column<int>(type: "int", nullable: true),
                    ResolvedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnRequests", x => x.ReturnRequestID);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Complaints_ComplaintID",
                        column: x => x.ComplaintID,
                        principalTable: "Complaints",
                        principalColumn: "ComplaintID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Deliveries_DeliveryID",
                        column: x => x.DeliveryID,
                        principalTable: "Deliveries",
                        principalColumn: "DeliveryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Deliveries_ReplacementDeliveryID",
                        column: x => x.ReplacementDeliveryID,
                        principalTable: "Deliveries",
                        principalColumn: "DeliveryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Employees_ResolvedByEmployeeID",
                        column: x => x.ResolvedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Orders_ReplacementOrderID",
                        column: x => x.ReplacementOrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_SupplierReturns_SupplierReturnID",
                        column: x => x.SupplierReturnID,
                        principalTable: "SupplierReturns",
                        principalColumn: "SupplierReturnID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryItems",
                columns: table => new
                {
                    DeliveryItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryID = table.Column<int>(type: "int", nullable: false),
                    PickListItemID = table.Column<int>(type: "int", nullable: true),
                    OrderItemID = table.Column<int>(type: "int", nullable: false),
                    InvoiceItemID = table.Column<int>(type: "int", nullable: true),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    BatchID = table.Column<int>(type: "int", nullable: true),
                    QuantityDispatched = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    QuantityDelivered = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    QuantityRejectedAtDelivery = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryItems", x => x.DeliveryItemID);
                    table.CheckConstraint("CK_DeliveryItems_Quantities", "[QuantityDispatched] > 0 AND [QuantityDelivered] >= 0 AND [QuantityRejectedAtDelivery] >= 0");
                    table.CheckConstraint("CK_DeliveryItems_Total", "[QuantityDelivered] + [QuantityRejectedAtDelivery] <= [QuantityDispatched]");
                    table.ForeignKey(
                        name: "FK_DeliveryItems_Batches_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batches",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryItems_Deliveries_DeliveryID",
                        column: x => x.DeliveryID,
                        principalTable: "Deliveries",
                        principalColumn: "DeliveryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryItems_InvoiceItems_InvoiceItemID",
                        column: x => x.InvoiceItemID,
                        principalTable: "InvoiceItems",
                        principalColumn: "InvoiceItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryItems_OrderItems_OrderItemID",
                        column: x => x.OrderItemID,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryItems_PickListItems_PickListItemID",
                        column: x => x.PickListItemID,
                        principalTable: "PickListItems",
                        principalColumn: "PickListItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditNotes",
                columns: table => new
                {
                    CreditNoteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditNoteNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    InvoiceID = table.Column<int>(type: "int", nullable: false),
                    ReturnRequestID = table.Column<int>(type: "int", nullable: false),
                    CreditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditNotes", x => x.CreditNoteID);
                    table.CheckConstraint("CK_CreditNotes_Amount", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_CreditNotes_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditNotes_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditNotes_ReturnRequests_ReturnRequestID",
                        column: x => x.ReturnRequestID,
                        principalTable: "ReturnRequests",
                        principalColumn: "ReturnRequestID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReturnItems",
                columns: table => new
                {
                    ReturnItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnRequestID = table.Column<int>(type: "int", nullable: false),
                    DeliveryItemID = table.Column<int>(type: "int", nullable: true),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    BatchID = table.Column<int>(type: "int", nullable: true),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ConditionStatus = table.Column<int>(type: "int", nullable: true),
                    InspectionResult = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Disposition = table.Column<int>(type: "int", nullable: false),
                    RestockQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    QuarantineQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    DamageQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReplacementQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnItems", x => x.ReturnItemID);
                    table.CheckConstraint("CK_ReturnItems_Disposition", "[RestockQuantity] + [QuarantineQuantity] + [DamageQuantity] + [ReplacementQuantity] = [ReceivedQuantity]");
                    table.CheckConstraint("CK_ReturnItems_Quantities", "[RequestedQuantity] > 0 AND [ApprovedQuantity] >= 0 AND [ReceivedQuantity] >= 0 AND [RestockQuantity] >= 0 AND [QuarantineQuantity] >= 0 AND [DamageQuantity] >= 0 AND [ReplacementQuantity] >= 0");
                    table.ForeignKey(
                        name: "FK_ReturnItems_Batches_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batches",
                        principalColumn: "BatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnItems_DeliveryItems_DeliveryItemID",
                        column: x => x.DeliveryItemID,
                        principalTable: "DeliveryItems",
                        principalColumn: "DeliveryItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnItems_ReturnRequests_ReturnRequestID",
                        column: x => x.ReturnRequestID,
                        principalTable: "ReturnRequests",
                        principalColumn: "ReturnRequestID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReactivationCases_RepeatOrderID",
                table: "ReactivationCases",
                column: "RepeatOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_DeliveryID",
                table: "Complaints",
                column: "DeliveryID");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_InvoiceID",
                table: "Complaints",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_OrderID",
                table: "Complaints",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_Status_SLADueAt",
                table: "Complaints",
                columns: new[] { "Status", "SLADueAt" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Complaints_Satisfaction",
                table: "Complaints",
                sql: "[SatisfactionScore] IS NULL OR ([SatisfactionScore] BETWEEN 1 AND 5)");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAttributions_CampaignID",
                table: "CampaignAttributions",
                column: "CampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAttributions_OrderID",
                table: "CampaignAttributions",
                column: "OrderID");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CampaignAttributions_Reference",
                table: "CampaignAttributions",
                sql: "[LeadID] IS NOT NULL OR [OpportunityID] IS NOT NULL OR [QuotationID] IS NOT NULL OR [OrderID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedOffers_AppliedByUserID",
                table: "AppliedOffers",
                column: "AppliedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedOffers_CampaignOfferID_OrderID_OrderItemID",
                table: "AppliedOffers",
                columns: new[] { "CampaignOfferID", "OrderID", "OrderItemID" });

            migrationBuilder.CreateIndex(
                name: "IX_AppliedOffers_FreeSKUID",
                table: "AppliedOffers",
                column: "FreeSKUID");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedOffers_OrderID",
                table: "AppliedOffers",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedOffers_OrderItemID",
                table: "AppliedOffers",
                column: "OrderItemID");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedOffers_QuotationID",
                table: "AppliedOffers",
                column: "QuotationID");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedOffers_QuotationItemID",
                table: "AppliedOffers",
                column: "QuotationItemID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_ClientID",
                table: "CreditNotes",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_CreditNoteNo",
                table: "CreditNotes",
                column: "CreditNoteNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_InvoiceID",
                table: "CreditNotes",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_ReturnRequestID",
                table: "CreditNotes",
                column: "ReturnRequestID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveredByEmployeeID",
                table: "Deliveries",
                column: "DeliveredByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryNo",
                table: "Deliveries",
                column: "DeliveryNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_InvoiceID",
                table: "Deliveries",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderID",
                table: "Deliveries",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_PickListID",
                table: "Deliveries",
                column: "PickListID");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_Status_PlannedDeliveryDate",
                table: "Deliveries",
                columns: new[] { "Status", "PlannedDeliveryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_WarehouseID",
                table: "Deliveries",
                column: "WarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_BatchID",
                table: "DeliveryItems",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_DeliveryID",
                table: "DeliveryItems",
                column: "DeliveryID");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_InvoiceItemID",
                table: "DeliveryItems",
                column: "InvoiceItemID");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_OrderItemID",
                table: "DeliveryItems",
                column: "OrderItemID");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_PickListItemID",
                table: "DeliveryItems",
                column: "PickListItemID");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_SKUID",
                table: "DeliveryItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceID_OrderItemID",
                table: "InvoiceItems",
                columns: new[] { "InvoiceID", "OrderItemID" });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_OrderItemID",
                table: "InvoiceItems",
                column: "OrderItemID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_SKUID",
                table: "InvoiceItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ClientID_Status_DueDate",
                table: "Invoices",
                columns: new[] { "ClientID", "Status", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNo",
                table: "Invoices",
                column: "InvoiceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_OrderID",
                table: "Invoices",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ModernTradePurchaseOrderItems_ModernTradePurchaseOrderID_ExternalItemCode",
                table: "ModernTradePurchaseOrderItems",
                columns: new[] { "ModernTradePurchaseOrderID", "ExternalItemCode" });

            migrationBuilder.CreateIndex(
                name: "IX_ModernTradePurchaseOrderItems_SKUID",
                table: "ModernTradePurchaseOrderItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_ModernTradePurchaseOrders_ClientID_PONumber",
                table: "ModernTradePurchaseOrders",
                columns: new[] { "ClientID", "PONumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModernTradePurchaseOrders_DuplicateHash",
                table: "ModernTradePurchaseOrders",
                column: "DuplicateHash",
                unique: true,
                filter: "[DuplicateHash] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ModernTradePurchaseOrders_UploadedByEmployeeID",
                table: "ModernTradePurchaseOrders",
                column: "UploadedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ModernTradePurchaseOrders_VerificationStatus_Status_ReceivedDate",
                table: "ModernTradePurchaseOrders",
                columns: new[] { "VerificationStatus", "Status", "ReceivedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ModernTradePurchaseOrders_VerifiedByEmployeeID",
                table: "ModernTradePurchaseOrders",
                column: "VerifiedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderID_SKUID",
                table: "OrderItems",
                columns: new[] { "OrderID", "SKUID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_SKUID",
                table: "OrderItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CampaignID",
                table: "Orders",
                column: "CampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientID_Status_OrderDate",
                table: "Orders",
                columns: new[] { "ClientID", "Status", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EmployeeID",
                table: "Orders",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ModernTradePurchaseOrderID",
                table: "Orders",
                column: "ModernTradePurchaseOrderID",
                unique: true,
                filter: "[ModernTradePurchaseOrderID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNo",
                table: "Orders",
                column: "OrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PriceListID",
                table: "Orders",
                column: "PriceListID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_QuotationID",
                table: "Orders",
                column: "QuotationID",
                unique: true,
                filter: "[QuotationID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_AllocatedByUserID",
                table: "PaymentAllocations",
                column: "AllocatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_InvoiceID",
                table: "PaymentAllocations",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_PaymentID_InvoiceID_AllocationType",
                table: "PaymentAllocations",
                columns: new[] { "PaymentID", "InvoiceID", "AllocationType" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_ReversalOfPaymentAllocationID",
                table: "PaymentAllocations",
                column: "ReversalOfPaymentAllocationID",
                unique: true,
                filter: "[ReversalOfPaymentAllocationID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ClientID_Status_PaymentDate",
                table: "Payments",
                columns: new[] { "ClientID", "Status", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentNo",
                table: "Payments",
                column: "PaymentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ReceivedByUserID",
                table: "Payments",
                column: "ReceivedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PickListItems_BatchID",
                table: "PickListItems",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_PickListItems_OrderItemID",
                table: "PickListItems",
                column: "OrderItemID");

            migrationBuilder.CreateIndex(
                name: "IX_PickListItems_PickedByEmployeeID",
                table: "PickListItems",
                column: "PickedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PickListItems_PickListID_StockReservationID",
                table: "PickListItems",
                columns: new[] { "PickListID", "StockReservationID" });

            migrationBuilder.CreateIndex(
                name: "IX_PickListItems_SKUID",
                table: "PickListItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_PickListItems_StockReservationID",
                table: "PickListItems",
                column: "StockReservationID");

            migrationBuilder.CreateIndex(
                name: "IX_PickLists_InvoiceID",
                table: "PickLists",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_PickLists_OrderID",
                table: "PickLists",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_PickLists_PickListNo",
                table: "PickLists",
                column: "PickListNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickLists_ReleasedByEmployeeID",
                table: "PickLists",
                column: "ReleasedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PickLists_VerifiedByEmployeeID",
                table: "PickLists",
                column: "VerifiedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PickLists_WarehouseID_Status_ReleasedAt",
                table: "PickLists",
                columns: new[] { "WarehouseID", "Status", "ReleasedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_BatchID",
                table: "ReturnItems",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_DeliveryItemID",
                table: "ReturnItems",
                column: "DeliveryItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_ReturnRequestID",
                table: "ReturnItems",
                column: "ReturnRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_SKUID",
                table: "ReturnItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ClientID_Status_RequestDate",
                table: "ReturnRequests",
                columns: new[] { "ClientID", "Status", "RequestDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ComplaintID",
                table: "ReturnRequests",
                column: "ComplaintID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_DeliveryID",
                table: "ReturnRequests",
                column: "DeliveryID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_InvoiceID",
                table: "ReturnRequests",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_OrderID",
                table: "ReturnRequests",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ReplacementDeliveryID",
                table: "ReturnRequests",
                column: "ReplacementDeliveryID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ReplacementOrderID",
                table: "ReturnRequests",
                column: "ReplacementOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ResolvedByEmployeeID",
                table: "ReturnRequests",
                column: "ResolvedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ReturnNo",
                table: "ReturnRequests",
                column: "ReturnNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_SupplierReturnID",
                table: "ReturnRequests",
                column: "SupplierReturnID");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignAttributions_Orders_OrderID",
                table: "CampaignAttributions",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Deliveries_DeliveryID",
                table: "Complaints",
                column: "DeliveryID",
                principalTable: "Deliveries",
                principalColumn: "DeliveryID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Invoices_InvoiceID",
                table: "Complaints",
                column: "InvoiceID",
                principalTable: "Invoices",
                principalColumn: "InvoiceID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Orders_OrderID",
                table: "Complaints",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReactivationCases_Orders_RepeatOrderID",
                table: "ReactivationCases",
                column: "RepeatOrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockReservations_OrderItems_OrderItemID",
                table: "StockReservations",
                column: "OrderItemID",
                principalTable: "OrderItems",
                principalColumn: "OrderItemID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignAttributions_Orders_OrderID",
                table: "CampaignAttributions");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Deliveries_DeliveryID",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Invoices_InvoiceID",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Orders_OrderID",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_ReactivationCases_Orders_RepeatOrderID",
                table: "ReactivationCases");

            migrationBuilder.DropForeignKey(
                name: "FK_StockReservations_OrderItems_OrderItemID",
                table: "StockReservations");

            migrationBuilder.DropTable(
                name: "AppliedOffers");

            migrationBuilder.DropTable(
                name: "CreditNotes");

            migrationBuilder.DropTable(
                name: "ModernTradePurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "PaymentAllocations");

            migrationBuilder.DropTable(
                name: "ReturnItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "DeliveryItems");

            migrationBuilder.DropTable(
                name: "ReturnRequests");

            migrationBuilder.DropTable(
                name: "InvoiceItems");

            migrationBuilder.DropTable(
                name: "PickListItems");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "PickLists");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ModernTradePurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_ReactivationCases_RepeatOrderID",
                table: "ReactivationCases");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_DeliveryID",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_InvoiceID",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_OrderID",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_Status_SLADueAt",
                table: "Complaints");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Complaints_Satisfaction",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_CampaignAttributions_CampaignID",
                table: "CampaignAttributions");

            migrationBuilder.DropIndex(
                name: "IX_CampaignAttributions_OrderID",
                table: "CampaignAttributions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CampaignAttributions_Reference",
                table: "CampaignAttributions");

            migrationBuilder.AlterColumn<string>(
                name: "ChurnReason",
                table: "ReactivationCases",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResolutionNote",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_Status_SLADueAt_AssignedEmployeeID",
                table: "Complaints",
                columns: new[] { "Status", "SLADueAt", "AssignedEmployeeID" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Complaints_Satisfaction",
                table: "Complaints",
                sql: "[SatisfactionScore] IS NULL OR ([SatisfactionScore] >= 1 AND [SatisfactionScore] <= 5)");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAttributions_CampaignID_AttributionType",
                table: "CampaignAttributions",
                columns: new[] { "CampaignID", "AttributionType" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_CampaignAttributions_Amount",
                table: "CampaignAttributions",
                sql: "[AttributedAmount] IS NULL OR [AttributedAmount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CampaignAttributions_Stage",
                table: "CampaignAttributions",
                sql: "[LeadID] IS NOT NULL OR [OpportunityID] IS NOT NULL OR [QuotationID] IS NOT NULL OR [OrderID] IS NOT NULL");
        }
    }
}
