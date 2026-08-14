using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _04_ProductPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    BrandID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    OwnerCompanyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsCustomerFacing = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Brands", x => x.BrandID);
                });

            migrationBuilder.CreateTable(
                name: "PriceLists",
                columns: table => new
                {
                    PriceListID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceListCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    PriceListName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    ClientSegmentID = table.Column<int>(type: "int", nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceLists", x => x.PriceListID);
                    table.CheckConstraint("CK_PriceLists_DateRange", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.ForeignKey(
                        name: "FK_PriceLists_ClientSegments_ClientSegmentID",
                        column: x => x.ClientSegmentID,
                        principalTable: "ClientSegments",
                        principalColumn: "ClientSegmentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    ProductCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentProductCategoryID = table.Column<int>(type: "int", nullable: true),
                    CategoryCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CategoryType = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ProductCategories", x => x.ProductCategoryID);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_ParentProductCategoryID",
                        column: x => x.ParentProductCategoryID,
                        principalTable: "ProductCategories",
                        principalColumn: "ProductCategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ProductCategoryID = table.Column<int>(type: "int", nullable: false),
                    BrandID = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiresBatch = table.Column<bool>(type: "bit", nullable: false),
                    RequiresExpiryDate = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Products", x => x.ProductID);
                    table.CheckConstraint("CK_Products_ExpiryRequiresBatch", "[RequiresExpiryDate] = 0 OR [RequiresBatch] = 1");
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandID",
                        column: x => x.BrandID,
                        principalTable: "Brands",
                        principalColumn: "BrandID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_ProductCategoryID",
                        column: x => x.ProductCategoryID,
                        principalTable: "ProductCategories",
                        principalColumn: "ProductCategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SKUs",
                columns: table => new
                {
                    SKUID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    SKUCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SKUName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MRP = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StandardTradePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_SKUs", x => x.SKUID);
                    table.CheckConstraint("CK_SKUs_MRP", "[MRP] >= 0");
                    table.CheckConstraint("CK_SKUs_TradePrice", "[StandardTradePrice] >= 0");
                    table.ForeignKey(
                        name: "FK_SKUs_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PriceListItems",
                columns: table => new
                {
                    PriceListItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceListID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaximumDiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MinimumOrderQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceListItems", x => x.PriceListItemID);
                    table.CheckConstraint("CK_PriceListItems_MaximumDiscount", "[MaximumDiscountPercent] >= 0 AND [MaximumDiscountPercent] <= 100");
                    table.CheckConstraint("CK_PriceListItems_MinimumOrderQuantity", "[MinimumOrderQuantity] IS NULL OR [MinimumOrderQuantity] > 0");
                    table.CheckConstraint("CK_PriceListItems_UnitPrice", "[UnitPrice] >= 0");
                    table.ForeignKey(
                        name: "FK_PriceListItems_PriceLists_PriceListID",
                        column: x => x.PriceListID,
                        principalTable: "PriceLists",
                        principalColumn: "PriceListID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceListItems_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StandardDiscountRules",
                columns: table => new
                {
                    StandardDiscountRuleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    ClientSegmentID = table.Column<int>(type: "int", nullable: true),
                    SKUID = table.Column<int>(type: "int", nullable: true),
                    ProductCategoryID = table.Column<int>(type: "int", nullable: true),
                    MinQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    MaxDiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardDiscountRules", x => x.StandardDiscountRuleID);
                    table.CheckConstraint("CK_StandardDiscountRules_DateRange", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.CheckConstraint("CK_StandardDiscountRules_MaxDiscount", "[MaxDiscountPercent] >= 0 AND [MaxDiscountPercent] <= 100");
                    table.CheckConstraint("CK_StandardDiscountRules_MinQuantity", "[MinQuantity] IS NULL OR [MinQuantity] > 0");
                    table.CheckConstraint("CK_StandardDiscountRules_ProductScope", "[SKUID] IS NULL OR [ProductCategoryID] IS NULL");
                    table.ForeignKey(
                        name: "FK_StandardDiscountRules_ClientSegments_ClientSegmentID",
                        column: x => x.ClientSegmentID,
                        principalTable: "ClientSegments",
                        principalColumn: "ClientSegmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StandardDiscountRules_ProductCategories_ProductCategoryID",
                        column: x => x.ProductCategoryID,
                        principalTable: "ProductCategories",
                        principalColumn: "ProductCategoryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StandardDiscountRules_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SamplingLogs_SKUID",
                table: "SamplingLogs",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_PriceListID",
                table: "Quotations",
                column: "PriceListID");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_SKUID",
                table: "QuotationItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_MarketObservations_SKUID",
                table: "MarketObservations",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_ProductCategoryID",
                table: "CampaignTargets",
                column: "ProductCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_SKUID",
                table: "CampaignTargets",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignOffers_FreeSKUID",
                table: "CampaignOffers",
                column: "FreeSKUID");

            migrationBuilder.CreateIndex(
                name: "IX_BPSellOutItems_SKUID",
                table: "BPSellOutItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_BrandCode",
                table: "Brands",
                column: "BrandCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_BrandName",
                table: "Brands",
                column: "BrandName");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItems_PriceListID_SKUID",
                table: "PriceListItems",
                columns: new[] { "PriceListID", "SKUID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItems_SKUID",
                table: "PriceListItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceLists_Channel_ClientSegmentID_Status_EffectiveFrom_EffectiveTo",
                table: "PriceLists",
                columns: new[] { "Channel", "ClientSegmentID", "Status", "EffectiveFrom", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_PriceLists_ClientSegmentID",
                table: "PriceLists",
                column: "ClientSegmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceLists_PriceListCode",
                table: "PriceLists",
                column: "PriceListCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryCode",
                table: "ProductCategories",
                column: "CategoryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentProductCategoryID_CategoryName",
                table: "ProductCategories",
                columns: new[] { "ParentProductCategoryID", "CategoryName" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandID_IsActive",
                table: "Products",
                columns: new[] { "BrandID", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCategoryID_IsActive",
                table: "Products",
                columns: new[] { "ProductCategoryID", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCode",
                table: "Products",
                column: "ProductCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SKUs_Barcode",
                table: "SKUs",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SKUs_ProductID_IsActive",
                table: "SKUs",
                columns: new[] { "ProductID", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_SKUs_SKUCode",
                table: "SKUs",
                column: "SKUCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StandardDiscountRules_Channel_ClientSegmentID_SKUID_ProductCategoryID_IsActive_EffectiveFrom_EffectiveTo",
                table: "StandardDiscountRules",
                columns: new[] { "Channel", "ClientSegmentID", "SKUID", "ProductCategoryID", "IsActive", "EffectiveFrom", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_StandardDiscountRules_ClientSegmentID",
                table: "StandardDiscountRules",
                column: "ClientSegmentID");

            migrationBuilder.CreateIndex(
                name: "IX_StandardDiscountRules_ProductCategoryID",
                table: "StandardDiscountRules",
                column: "ProductCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_StandardDiscountRules_SKUID",
                table: "StandardDiscountRules",
                column: "SKUID");

            migrationBuilder.AddForeignKey(
                name: "FK_BPSellOutItems_SKUs_SKUID",
                table: "BPSellOutItems",
                column: "SKUID",
                principalTable: "SKUs",
                principalColumn: "SKUID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignOffers_SKUs_FreeSKUID",
                table: "CampaignOffers",
                column: "FreeSKUID",
                principalTable: "SKUs",
                principalColumn: "SKUID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignTargets_ProductCategories_ProductCategoryID",
                table: "CampaignTargets",
                column: "ProductCategoryID",
                principalTable: "ProductCategories",
                principalColumn: "ProductCategoryID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignTargets_SKUs_SKUID",
                table: "CampaignTargets",
                column: "SKUID",
                principalTable: "SKUs",
                principalColumn: "SKUID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketObservations_SKUs_SKUID",
                table: "MarketObservations",
                column: "SKUID",
                principalTable: "SKUs",
                principalColumn: "SKUID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationItems_SKUs_SKUID",
                table: "QuotationItems",
                column: "SKUID",
                principalTable: "SKUs",
                principalColumn: "SKUID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_PriceLists_PriceListID",
                table: "Quotations",
                column: "PriceListID",
                principalTable: "PriceLists",
                principalColumn: "PriceListID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SamplingLogs_SKUs_SKUID",
                table: "SamplingLogs",
                column: "SKUID",
                principalTable: "SKUs",
                principalColumn: "SKUID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BPSellOutItems_SKUs_SKUID",
                table: "BPSellOutItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CampaignOffers_SKUs_FreeSKUID",
                table: "CampaignOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_CampaignTargets_ProductCategories_ProductCategoryID",
                table: "CampaignTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_CampaignTargets_SKUs_SKUID",
                table: "CampaignTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketObservations_SKUs_SKUID",
                table: "MarketObservations");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationItems_SKUs_SKUID",
                table: "QuotationItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_PriceLists_PriceListID",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_SamplingLogs_SKUs_SKUID",
                table: "SamplingLogs");

            migrationBuilder.DropTable(
                name: "PriceListItems");

            migrationBuilder.DropTable(
                name: "StandardDiscountRules");

            migrationBuilder.DropTable(
                name: "PriceLists");

            migrationBuilder.DropTable(
                name: "SKUs");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_SamplingLogs_SKUID",
                table: "SamplingLogs");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_PriceListID",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_QuotationItems_SKUID",
                table: "QuotationItems");

            migrationBuilder.DropIndex(
                name: "IX_MarketObservations_SKUID",
                table: "MarketObservations");

            migrationBuilder.DropIndex(
                name: "IX_CampaignTargets_ProductCategoryID",
                table: "CampaignTargets");

            migrationBuilder.DropIndex(
                name: "IX_CampaignTargets_SKUID",
                table: "CampaignTargets");

            migrationBuilder.DropIndex(
                name: "IX_CampaignOffers_FreeSKUID",
                table: "CampaignOffers");

            migrationBuilder.DropIndex(
                name: "IX_BPSellOutItems_SKUID",
                table: "BPSellOutItems");
        }
    }
}
