using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _03_MarketingField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    CampaignID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CampaignTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Objective = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByEmployeeID = table.Column<int>(type: "int", nullable: false),
                    ActualExpense = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.CampaignID);
                    table.CheckConstraint("CK_Campaigns_Budget", "[Budget] >= 0 AND [ActualExpense] >= 0");
                    table.CheckConstraint("CK_Campaigns_DateRange", "[EndDate] >= [StartDate]");
                    table.ForeignKey(
                        name: "FK_Campaigns_Employees_CreatedByEmployeeID",
                        column: x => x.CreatedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignAttributions",
                columns: table => new
                {
                    CampaignAttributionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignID = table.Column<int>(type: "int", nullable: false),
                    LeadID = table.Column<int>(type: "int", nullable: true),
                    OpportunityID = table.Column<int>(type: "int", nullable: true),
                    QuotationID = table.Column<int>(type: "int", nullable: true),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    AttributionType = table.Column<int>(type: "int", nullable: false),
                    WeightPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AttributedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignAttributions", x => x.CampaignAttributionID);
                    table.CheckConstraint("CK_CampaignAttributions_Amount", "[AttributedAmount] IS NULL OR [AttributedAmount] >= 0");
                    table.CheckConstraint("CK_CampaignAttributions_Stage", "[LeadID] IS NOT NULL OR [OpportunityID] IS NOT NULL OR [QuotationID] IS NOT NULL OR [OrderID] IS NOT NULL");
                    table.CheckConstraint("CK_CampaignAttributions_Weight", "[WeightPercent] >= 0 AND [WeightPercent] <= 100");
                    table.ForeignKey(
                        name: "FK_CampaignAttributions_Campaigns_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignAttributions_Leads_LeadID",
                        column: x => x.LeadID,
                        principalTable: "Leads",
                        principalColumn: "LeadID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignAttributions_Opportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "Opportunities",
                        principalColumn: "OpportunityID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignAttributions_Quotations_QuotationID",
                        column: x => x.QuotationID,
                        principalTable: "Quotations",
                        principalColumn: "QuotationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignExpenses",
                columns: table => new
                {
                    CampaignExpenseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignID = table.Column<int>(type: "int", nullable: false),
                    ExpenseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpenseCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VendorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignExpenses", x => x.CampaignExpenseID);
                    table.CheckConstraint("CK_CampaignExpenses_Amount", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_CampaignExpenses_Campaigns_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignOffers",
                columns: table => new
                {
                    CampaignOfferID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignID = table.Column<int>(type: "int", nullable: false),
                    OfferCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    OfferType = table.Column<int>(type: "int", nullable: false),
                    RuleJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    FreeSKUID = table.Column<int>(type: "int", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    PerClientLimit = table.Column<int>(type: "int", nullable: true),
                    IsStackable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignOffers", x => x.CampaignOfferID);
                    table.CheckConstraint("CK_CampaignOffers_DiscountValue", "[DiscountValue] IS NULL OR [DiscountValue] >= 0");
                    table.CheckConstraint("CK_CampaignOffers_PerClientLimit", "[PerClientLimit] IS NULL OR [PerClientLimit] > 0");
                    table.CheckConstraint("CK_CampaignOffers_UsageLimit", "[UsageLimit] IS NULL OR [UsageLimit] > 0");
                    table.ForeignKey(
                        name: "FK_CampaignOffers_Campaigns_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignTargets",
                columns: table => new
                {
                    CampaignTargetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignID = table.Column<int>(type: "int", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    AreaID = table.Column<int>(type: "int", nullable: true),
                    ClientSegmentID = table.Column<int>(type: "int", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: true),
                    ProductCategoryID = table.Column<int>(type: "int", nullable: true),
                    SKUID = table.Column<int>(type: "int", nullable: true),
                    TargetValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignTargets", x => x.CampaignTargetID);
                    table.CheckConstraint("CK_CampaignTargets_OneReference", "(CASE WHEN [RegionID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [AreaID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ClientSegmentID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ClientID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ProductCategoryID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [SKUID] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.CheckConstraint("CK_CampaignTargets_TargetValue", "[TargetValue] IS NULL OR [TargetValue] >= 0");
                    table.CheckConstraint("CK_CampaignTargets_TypeReference", "([TargetType] = 1 AND [RegionID] IS NOT NULL) OR ([TargetType] = 2 AND [AreaID] IS NOT NULL) OR ([TargetType] = 3 AND [ClientSegmentID] IS NOT NULL) OR ([TargetType] = 4 AND [ClientID] IS NOT NULL) OR ([TargetType] = 5 AND [ProductCategoryID] IS NOT NULL) OR ([TargetType] = 6 AND [SKUID] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_CampaignTargets_Areas_AreaID",
                        column: x => x.AreaID,
                        principalTable: "Areas",
                        principalColumn: "AreaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_Campaigns_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_ClientSegments_ClientSegmentID",
                        column: x => x.ClientSegmentID,
                        principalTable: "ClientSegments",
                        principalColumn: "ClientSegmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignTargets_Regions_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    VisitID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    RouteID = table.Column<int>(type: "int", nullable: true),
                    CampaignID = table.Column<int>(type: "int", nullable: true),
                    VisitType = table.Column<int>(type: "int", nullable: false),
                    CheckInAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckInGPSLat = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false),
                    CheckInGPSLng = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false),
                    CheckOutGPSLat = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    CheckOutGPSLng = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    AccuracyMeters = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IsSuspiciousLocation = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.VisitID);
                    table.CheckConstraint("CK_Visits_Accuracy", "[AccuracyMeters] IS NULL OR [AccuracyMeters] > 0");
                    table.CheckConstraint("CK_Visits_CheckInLatitude", "[CheckInGPSLat] >= -90 AND [CheckInGPSLat] <= 90");
                    table.CheckConstraint("CK_Visits_CheckInLongitude", "[CheckInGPSLng] >= -180 AND [CheckInGPSLng] <= 180");
                    table.CheckConstraint("CK_Visits_Checkout", "[CheckOutAt] IS NULL OR [CheckOutAt] >= [CheckInAt]");
                    table.CheckConstraint("CK_Visits_CheckOutLatitude", "[CheckOutGPSLat] IS NULL OR ([CheckOutGPSLat] >= -90 AND [CheckOutGPSLat] <= 90)");
                    table.CheckConstraint("CK_Visits_CheckOutLongitude", "[CheckOutGPSLng] IS NULL OR ([CheckOutGPSLng] >= -180 AND [CheckOutGPSLng] <= 180)");
                    table.ForeignKey(
                        name: "FK_Visits_Campaigns_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visits_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visits_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visits_Routes_RouteID",
                        column: x => x.RouteID,
                        principalTable: "Routes",
                        principalColumn: "RouteID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BPSellOuts",
                columns: table => new
                {
                    BPSellOutID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    VisitID = table.Column<int>(type: "int", nullable: true),
                    CampaignID = table.Column<int>(type: "int", nullable: true),
                    SellOutDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GPSLat = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    GPSLng = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    VerifiedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BPSellOuts", x => x.BPSellOutID);
                    table.CheckConstraint("CK_BPSellOuts_Latitude", "[GPSLat] IS NULL OR ([GPSLat] >= -90 AND [GPSLat] <= 90)");
                    table.CheckConstraint("CK_BPSellOuts_Longitude", "[GPSLng] IS NULL OR ([GPSLng] >= -180 AND [GPSLng] <= 180)");
                    table.CheckConstraint("CK_BPSellOuts_Totals", "[TotalQuantity] > 0 AND [TotalValue] >= 0");
                    table.ForeignKey(
                        name: "FK_BPSellOuts_Campaigns_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BPSellOuts_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BPSellOuts_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BPSellOuts_Employees_VerifiedByEmployeeID",
                        column: x => x.VerifiedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BPSellOuts_Visits_VisitID",
                        column: x => x.VisitID,
                        principalTable: "Visits",
                        principalColumn: "VisitID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: true),
                    LeadID = table.Column<int>(type: "int", nullable: true),
                    CampaignID = table.Column<int>(type: "int", nullable: true),
                    VisitID = table.Column<int>(type: "int", nullable: true),
                    SubmittedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    FeedbackType = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFollowUpRequired = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.FeedbackID);
                    table.CheckConstraint("CK_Feedbacks_Party", "[ClientID] IS NOT NULL OR [LeadID] IS NOT NULL");
                    table.CheckConstraint("CK_Feedbacks_Rating", "[Rating] IS NULL OR ([Rating] >= 1 AND [Rating] <= 5)");
                    table.ForeignKey(
                        name: "FK_Feedbacks_Campaigns_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Employees_SubmittedByEmployeeID",
                        column: x => x.SubmittedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Leads_LeadID",
                        column: x => x.LeadID,
                        principalTable: "Leads",
                        principalColumn: "LeadID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Visits_VisitID",
                        column: x => x.VisitID,
                        principalTable: "Visits",
                        principalColumn: "VisitID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketObservations",
                columns: table => new
                {
                    MarketObservationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitID = table.Column<int>(type: "int", nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    ObservationType = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: true),
                    AvailabilityStatus = table.Column<int>(type: "int", nullable: true),
                    FacingCount = table.Column<int>(type: "int", nullable: true),
                    PlanogramScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    DisplayScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    CompetitorBrand = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CompetitorProduct = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompetitorPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CompetitorOffer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketObservations", x => x.MarketObservationID);
                    table.CheckConstraint("CK_MarketObservations_CompetitorPrice", "[CompetitorPrice] IS NULL OR [CompetitorPrice] >= 0");
                    table.CheckConstraint("CK_MarketObservations_Display", "[DisplayScore] IS NULL OR ([DisplayScore] >= 0 AND [DisplayScore] <= 100)");
                    table.CheckConstraint("CK_MarketObservations_Facing", "[FacingCount] IS NULL OR [FacingCount] >= 0");
                    table.CheckConstraint("CK_MarketObservations_Planogram", "[PlanogramScore] IS NULL OR ([PlanogramScore] >= 0 AND [PlanogramScore] <= 100)");
                    table.ForeignKey(
                        name: "FK_MarketObservations_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketObservations_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketObservations_Visits_VisitID",
                        column: x => x.VisitID,
                        principalTable: "Visits",
                        principalColumn: "VisitID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SamplingLogs",
                columns: table => new
                {
                    SamplingLogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitID = table.Column<int>(type: "int", nullable: true),
                    CampaignID = table.Column<int>(type: "int", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: true),
                    LeadID = table.Column<int>(type: "int", nullable: true),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    IssuedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ConsumedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReturnedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    DamagedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    SampleDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FeedbackSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Outcome = table.Column<int>(type: "int", nullable: false),
                    FollowUpRequired = table.Column<bool>(type: "bit", nullable: false),
                    IssueStockMovementID = table.Column<int>(type: "int", nullable: true),
                    ReturnStockMovementID = table.Column<int>(type: "int", nullable: true),
                    DamageStockMovementID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamplingLogs", x => x.SamplingLogID);
                    table.CheckConstraint("CK_SamplingLogs_Balance", "[IssuedQuantity] = [ConsumedQuantity] + [ReturnedQuantity] + [DamagedQuantity]");
                    table.CheckConstraint("CK_SamplingLogs_Party", "[ClientID] IS NOT NULL OR [LeadID] IS NOT NULL");
                    table.CheckConstraint("CK_SamplingLogs_Quantities", "[IssuedQuantity] > 0 AND [ConsumedQuantity] >= 0 AND [ReturnedQuantity] >= 0 AND [DamagedQuantity] >= 0");
                    table.ForeignKey(
                        name: "FK_SamplingLogs_Campaigns_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SamplingLogs_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SamplingLogs_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SamplingLogs_Leads_LeadID",
                        column: x => x.LeadID,
                        principalTable: "Leads",
                        principalColumn: "LeadID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SamplingLogs_Visits_VisitID",
                        column: x => x.VisitID,
                        principalTable: "Visits",
                        principalColumn: "VisitID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BPSellOutItems",
                columns: table => new
                {
                    BPSellOutItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BPSellOutID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    QuantitySold = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitSellingPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LineValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BPSellOutItems", x => x.BPSellOutItemID);
                    table.CheckConstraint("CK_BPSellOutItems_Price", "[UnitSellingPrice] IS NULL OR [UnitSellingPrice] >= 0");
                    table.CheckConstraint("CK_BPSellOutItems_Quantity", "[QuantitySold] > 0");
                    table.CheckConstraint("CK_BPSellOutItems_Value", "[LineValue] IS NULL OR [LineValue] >= 0");
                    table.ForeignKey(
                        name: "FK_BPSellOutItems_BPSellOuts_BPSellOutID",
                        column: x => x.BPSellOutID,
                        principalTable: "BPSellOuts",
                        principalColumn: "BPSellOutID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReactivationCases_CampaignID",
                table: "ReactivationCases",
                column: "CampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CampaignID",
                table: "Quotations",
                column: "CampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_CampaignID",
                table: "Opportunities",
                column: "CampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_SourceCampaignID",
                table: "Leads",
                column: "SourceCampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_BPSellOutItems_BPSellOutID_SKUID",
                table: "BPSellOutItems",
                columns: new[] { "BPSellOutID", "SKUID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BPSellOuts_CampaignID",
                table: "BPSellOuts",
                column: "CampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_BPSellOuts_ClientID_SellOutDate",
                table: "BPSellOuts",
                columns: new[] { "ClientID", "SellOutDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BPSellOuts_EmployeeID",
                table: "BPSellOuts",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_BPSellOuts_VerificationStatus_SellOutDate",
                table: "BPSellOuts",
                columns: new[] { "VerificationStatus", "SellOutDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BPSellOuts_VerifiedByEmployeeID",
                table: "BPSellOuts",
                column: "VerifiedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_BPSellOuts_VisitID",
                table: "BPSellOuts",
                column: "VisitID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAttributions_CampaignID_AttributionType",
                table: "CampaignAttributions",
                columns: new[] { "CampaignID", "AttributionType" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAttributions_LeadID",
                table: "CampaignAttributions",
                column: "LeadID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAttributions_OpportunityID",
                table: "CampaignAttributions",
                column: "OpportunityID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignAttributions_QuotationID",
                table: "CampaignAttributions",
                column: "QuotationID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignExpenses_CampaignID_ExpenseDate_Status",
                table: "CampaignExpenses",
                columns: new[] { "CampaignID", "ExpenseDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignOffers_CampaignID_IsActive_Priority",
                table: "CampaignOffers",
                columns: new[] { "CampaignID", "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignOffers_CampaignID_OfferCode",
                table: "CampaignOffers",
                columns: new[] { "CampaignID", "OfferCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CampaignCode",
                table: "Campaigns",
                column: "CampaignCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CreatedByEmployeeID",
                table: "Campaigns",
                column: "CreatedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Status_StartDate_EndDate",
                table: "Campaigns",
                columns: new[] { "Status", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_AreaID",
                table: "CampaignTargets",
                column: "AreaID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_CampaignID_TargetType",
                table: "CampaignTargets",
                columns: new[] { "CampaignID", "TargetType" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_ClientID",
                table: "CampaignTargets",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_ClientSegmentID",
                table: "CampaignTargets",
                column: "ClientSegmentID");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignTargets_RegionID",
                table: "CampaignTargets",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_CampaignID_SubmittedAt",
                table: "Feedbacks",
                columns: new[] { "CampaignID", "SubmittedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_ClientID",
                table: "Feedbacks",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_IsFollowUpRequired_SubmittedAt",
                table: "Feedbacks",
                columns: new[] { "IsFollowUpRequired", "SubmittedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_LeadID",
                table: "Feedbacks",
                column: "LeadID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_SubmittedByEmployeeID",
                table: "Feedbacks",
                column: "SubmittedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_VisitID",
                table: "Feedbacks",
                column: "VisitID");

            migrationBuilder.CreateIndex(
                name: "IX_MarketObservations_ClientID_ObservationType",
                table: "MarketObservations",
                columns: new[] { "ClientID", "ObservationType" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketObservations_EmployeeID_CreatedAt",
                table: "MarketObservations",
                columns: new[] { "EmployeeID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketObservations_VisitID",
                table: "MarketObservations",
                column: "VisitID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplingLogs_CampaignID_SampleDate",
                table: "SamplingLogs",
                columns: new[] { "CampaignID", "SampleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SamplingLogs_ClientID",
                table: "SamplingLogs",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplingLogs_EmployeeID_SampleDate",
                table: "SamplingLogs",
                columns: new[] { "EmployeeID", "SampleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SamplingLogs_LeadID",
                table: "SamplingLogs",
                column: "LeadID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplingLogs_VisitID",
                table: "SamplingLogs",
                column: "VisitID");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_CampaignID",
                table: "Visits",
                column: "CampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_ClientID_CheckInAt",
                table: "Visits",
                columns: new[] { "ClientID", "CheckInAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_EmployeeID_CheckInAt",
                table: "Visits",
                columns: new[] { "EmployeeID", "CheckInAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_RouteID",
                table: "Visits",
                column: "RouteID");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_Status_CheckInAt",
                table: "Visits",
                columns: new[] { "Status", "CheckInAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Campaigns_SourceCampaignID",
                table: "Leads",
                column: "SourceCampaignID",
                principalTable: "Campaigns",
                principalColumn: "CampaignID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Campaigns_CampaignID",
                table: "Opportunities",
                column: "CampaignID",
                principalTable: "Campaigns",
                principalColumn: "CampaignID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Campaigns_CampaignID",
                table: "Quotations",
                column: "CampaignID",
                principalTable: "Campaigns",
                principalColumn: "CampaignID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReactivationCases_Campaigns_CampaignID",
                table: "ReactivationCases",
                column: "CampaignID",
                principalTable: "Campaigns",
                principalColumn: "CampaignID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Campaigns_SourceCampaignID",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Campaigns_CampaignID",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Campaigns_CampaignID",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReactivationCases_Campaigns_CampaignID",
                table: "ReactivationCases");

            migrationBuilder.DropTable(
                name: "BPSellOutItems");

            migrationBuilder.DropTable(
                name: "CampaignAttributions");

            migrationBuilder.DropTable(
                name: "CampaignExpenses");

            migrationBuilder.DropTable(
                name: "CampaignOffers");

            migrationBuilder.DropTable(
                name: "CampaignTargets");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "MarketObservations");

            migrationBuilder.DropTable(
                name: "SamplingLogs");

            migrationBuilder.DropTable(
                name: "BPSellOuts");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_ReactivationCases_CampaignID",
                table: "ReactivationCases");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_CampaignID",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_CampaignID",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Leads_SourceCampaignID",
                table: "Leads");
        }
    }
}
