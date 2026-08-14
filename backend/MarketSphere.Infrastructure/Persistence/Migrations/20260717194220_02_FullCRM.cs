using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _02_FullCRM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RouteOutlets_RouteID_ClientID_EffectiveFrom",
                table: "RouteOutlets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RouteOutlets_ClientID",
                table: "RouteOutlets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RouteOutlets_EffectiveDates",
                table: "RouteOutlets");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClientType = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GPSLat = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    GPSLng = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    AreaID = table.Column<int>(type: "int", nullable: true),
                    TerritoryID = table.Column<int>(type: "int", nullable: true),
                    LifecycleStatus = table.Column<int>(type: "int", nullable: false),
                    RiskStatus = table.Column<int>(type: "int", nullable: false),
                    LastOrderAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_Clients", x => x.ClientID);
                    table.ForeignKey(
                        name: "FK_Clients_Areas_AreaID",
                        column: x => x.AreaID,
                        principalTable: "Areas",
                        principalColumn: "AreaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_Regions_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_Territories_TerritoryID",
                        column: x => x.TerritoryID,
                        principalTable: "Territories",
                        principalColumn: "TerritoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientSegments",
                columns: table => new
                {
                    ClientSegmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SegmentCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SegmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SegmentType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystemSegment = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ClientSegments", x => x.ClientSegmentID);
                });

            migrationBuilder.CreateTable(
                name: "DuplicateReviewCases",
                columns: table => new
                {
                    DuplicateReviewCaseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceEntityID = table.Column<int>(type: "int", nullable: false),
                    MatchedEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatchedEntityID = table.Column<int>(type: "int", nullable: false),
                    MatchScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MatchReasonsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResolutionType = table.Column<int>(type: "int", nullable: true),
                    SurvivorEntityID = table.Column<int>(type: "int", nullable: true),
                    ResolvedByUserID = table.Column<int>(type: "int", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuplicateReviewCases", x => x.DuplicateReviewCaseID);
                    table.CheckConstraint("CK_DuplicateReviewCases_NotSelf", "NOT ([SourceEntityType] = [MatchedEntityType] AND [SourceEntityID] = [MatchedEntityID])");
                });

            migrationBuilder.CreateTable(
                name: "LeadScoreRules",
                columns: table => new
                {
                    LeadScoreRuleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ConditionType = table.Column<int>(type: "int", nullable: false),
                    Operator = table.Column<int>(type: "int", nullable: false),
                    ComparisonValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ScoreValue = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_LeadScoreRules", x => x.LeadScoreRuleID);
                    table.CheckConstraint("CK_LeadScoreRules_DateRange", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                });

            migrationBuilder.CreateTable(
                name: "ClientContacts",
                columns: table => new
                {
                    ClientContactID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientContacts", x => x.ClientContactID);
                    table.ForeignKey(
                        name: "FK_ClientContacts_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientCreditProfiles",
                columns: table => new
                {
                    ClientCreditProfileID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreditDays = table.Column<int>(type: "int", nullable: false),
                    CurrentDue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    BlockReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCreditProfiles", x => x.ClientCreditProfileID);
                    table.CheckConstraint("CK_ClientCreditProfiles_CreditDays", "[CreditDays] >= 0");
                    table.CheckConstraint("CK_ClientCreditProfiles_CreditLimit", "[CreditLimit] >= 0");
                    table.CheckConstraint("CK_ClientCreditProfiles_CurrentDue", "[CurrentDue] >= 0");
                    table.ForeignKey(
                        name: "FK_ClientCreditProfiles_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    ComplaintID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComplaintNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    InvoiceID = table.Column<int>(type: "int", nullable: true),
                    DeliveryID = table.Column<int>(type: "int", nullable: true),
                    ComplaintCategory = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedEmployeeID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SLADueAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SatisfactionScore = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.ComplaintID);
                    table.CheckConstraint("CK_Complaints_Satisfaction", "[SatisfactionScore] IS NULL OR ([SatisfactionScore] >= 1 AND [SatisfactionScore] <= 5)");
                    table.ForeignKey(
                        name: "FK_Complaints_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaints_Employees_AssignedEmployeeID",
                        column: x => x.AssignedEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    LeadID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LeadName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BusinessName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Source = table.Column<int>(type: "int", nullable: false),
                    SourceCampaignID = table.Column<int>(type: "int", nullable: true),
                    AssignedEmployeeID = table.Column<int>(type: "int", nullable: true),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    ProductInterest = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CurrentScore = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    NextFollowUpAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LostReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReactivationAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConvertedClientID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.LeadID);
                    table.CheckConstraint("CK_Leads_CurrentScore", "[CurrentScore] >= 0 AND [CurrentScore] <= 100");
                    table.ForeignKey(
                        name: "FK_Leads_Clients_ConvertedClientID",
                        column: x => x.ConvertedClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leads_Employees_AssignedEmployeeID",
                        column: x => x.AssignedEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leads_Regions_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReactivationCases",
                columns: table => new
                {
                    ReactivationCaseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    InactiveAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChurnReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CampaignID = table.Column<int>(type: "int", nullable: true),
                    AssignedEmployeeID = table.Column<int>(type: "int", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReactivationResult = table.Column<int>(type: "int", nullable: true),
                    ReactivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RepeatOrderID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactivationCases", x => x.ReactivationCaseID);
                    table.ForeignKey(
                        name: "FK_ReactivationCases_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReactivationCases_Employees_AssignedEmployeeID",
                        column: x => x.AssignedEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientSegmentAssignments",
                columns: table => new
                {
                    ClientSegmentAssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    ClientSegmentID = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedByUserID = table.Column<int>(type: "int", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSegmentAssignments", x => x.ClientSegmentAssignmentID);
                    table.CheckConstraint("CK_ClientSegmentAssignments_DateRange", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [AssignedAt]");
                    table.ForeignKey(
                        name: "FK_ClientSegmentAssignments_ClientSegments_ClientSegmentID",
                        column: x => x.ClientSegmentID,
                        principalTable: "ClientSegments",
                        principalColumn: "ClientSegmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientSegmentAssignments_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientSegmentAssignments_Users_AssignedByUserID",
                        column: x => x.AssignedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    OpportunityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LeadID = table.Column<int>(type: "int", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: true),
                    CampaignID = table.Column<int>(type: "int", nullable: true),
                    OwnerEmployeeID = table.Column<int>(type: "int", nullable: false),
                    OpportunityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    ExpectedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProbabilityPercent = table.Column<int>(type: "int", nullable: false),
                    ExpectedCloseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Competitor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LostReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WonAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.OpportunityID);
                    table.CheckConstraint("CK_Opportunities_Probability", "[ProbabilityPercent] >= 0 AND [ProbabilityPercent] <= 100");
                    table.CheckConstraint("CK_Opportunities_Value", "[ExpectedValue] >= 0");
                    table.ForeignKey(
                        name: "FK_Opportunities_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunities_Employees_OwnerEmployeeID",
                        column: x => x.OwnerEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunities_Leads_LeadID",
                        column: x => x.LeadID,
                        principalTable: "Leads",
                        principalColumn: "LeadID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CRMActivities",
                columns: table => new
                {
                    CRMActivityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadID = table.Column<int>(type: "int", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: true),
                    OpportunityID = table.Column<int>(type: "int", nullable: true),
                    ActivityType = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledStartAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledEndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LocationOrMeetingLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Agenda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityStatus = table.Column<int>(type: "int", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextActionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMActivities", x => x.CRMActivityID);
                    table.CheckConstraint("CK_CRMActivities_RelatedEntity", "[LeadID] IS NOT NULL OR [ClientID] IS NOT NULL OR [OpportunityID] IS NOT NULL");
                    table.CheckConstraint("CK_CRMActivities_Schedule", "[ScheduledEndAt] IS NULL OR [ScheduledStartAt] IS NULL OR [ScheduledEndAt] >= [ScheduledStartAt]");
                    table.ForeignKey(
                        name: "FK_CRMActivities_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CRMActivities_Employees_PerformedByEmployeeID",
                        column: x => x.PerformedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CRMActivities_Leads_LeadID",
                        column: x => x.LeadID,
                        principalTable: "Leads",
                        principalColumn: "LeadID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CRMActivities_Opportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "Opportunities",
                        principalColumn: "OpportunityID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CRMTasks",
                columns: table => new
                {
                    CRMTaskID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadID = table.Column<int>(type: "int", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: true),
                    OpportunityID = table.Column<int>(type: "int", nullable: true),
                    ComplaintID = table.Column<int>(type: "int", nullable: true),
                    ReactivationCaseID = table.Column<int>(type: "int", nullable: true),
                    AssignedEmployeeID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    DueAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReminderAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecurrenceRule = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EscalatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMTasks", x => x.CRMTaskID);
                    table.CheckConstraint("CK_CRMTasks_CompletedAt", "[Status] <> 3 OR [CompletedAt] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_CRMTasks_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CRMTasks_Complaints_ComplaintID",
                        column: x => x.ComplaintID,
                        principalTable: "Complaints",
                        principalColumn: "ComplaintID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CRMTasks_Employees_AssignedEmployeeID",
                        column: x => x.AssignedEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CRMTasks_Leads_LeadID",
                        column: x => x.LeadID,
                        principalTable: "Leads",
                        principalColumn: "LeadID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CRMTasks_Opportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "Opportunities",
                        principalColumn: "OpportunityID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CRMTasks_ReactivationCases_ReactivationCaseID",
                        column: x => x.ReactivationCaseID,
                        principalTable: "ReactivationCases",
                        principalColumn: "ReactivationCaseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    QuotationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RootQuotationID = table.Column<int>(type: "int", nullable: true),
                    VersionNo = table.Column<int>(type: "int", nullable: false),
                    QuotationNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    OpportunityID = table.Column<int>(type: "int", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    CampaignID = table.Column<int>(type: "int", nullable: true),
                    PriceListID = table.Column<int>(type: "int", nullable: true),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidUntil = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Terms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.QuotationID);
                    table.CheckConstraint("CK_Quotations_Amounts", "[GrossAmount] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [NetAmount] >= 0");
                    table.CheckConstraint("CK_Quotations_DateRange", "[ValidUntil] >= [ValidFrom]");
                    table.ForeignKey(
                        name: "FK_Quotations_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotations_Opportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "Opportunities",
                        principalColumn: "OpportunityID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotations_Quotations_RootQuotationID",
                        column: x => x.RootQuotationID,
                        principalTable: "Quotations",
                        principalColumn: "QuotationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CRMActivityParticipants",
                columns: table => new
                {
                    CRMActivityParticipantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CRMActivityID = table.Column<int>(type: "int", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: true),
                    ClientContactID = table.Column<int>(type: "int", nullable: true),
                    ExternalName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ExternalEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ParticipantRole = table.Column<int>(type: "int", nullable: true),
                    AttendanceStatus = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMActivityParticipants", x => x.CRMActivityParticipantID);
                    table.CheckConstraint("CK_CRMActivityParticipants_Identity", "[EmployeeID] IS NOT NULL OR [ClientContactID] IS NOT NULL OR [ExternalName] IS NOT NULL OR [ExternalEmail] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_CRMActivityParticipants_CRMActivities_CRMActivityID",
                        column: x => x.CRMActivityID,
                        principalTable: "CRMActivities",
                        principalColumn: "CRMActivityID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CRMActivityParticipants_ClientContacts_ClientContactID",
                        column: x => x.ClientContactID,
                        principalTable: "ClientContacts",
                        principalColumn: "ClientContactID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CRMActivityParticipants_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationItems",
                columns: table => new
                {
                    QuotationItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationItems", x => x.QuotationItemID);
                    table.CheckConstraint("CK_QuotationItems_Amounts", "[UnitPrice] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [LineTotal] >= 0");
                    table.CheckConstraint("CK_QuotationItems_Discount", "[DiscountPercent] >= 0 AND [DiscountPercent] <= 100");
                    table.CheckConstraint("CK_QuotationItems_Quantity", "[Quantity] > 0");
                    table.ForeignKey(
                        name: "FK_QuotationItems_Quotations_QuotationID",
                        column: x => x.QuotationID,
                        principalTable: "Quotations",
                        principalColumn: "QuotationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteOutlets_ClientID",
                table: "RouteOutlets",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_RouteOutlets_RouteID_ClientID",
                table: "RouteOutlets",
                columns: new[] { "RouteID", "ClientID" },
                unique: true,
                filter: "[EffectiveTo] IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RouteOutlets_DateRange",
                table: "RouteOutlets",
                sql: "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RouteOutlets_VisitFrequency",
                table: "RouteOutlets",
                sql: "[VisitFrequency] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_ClientID",
                table: "ClientContacts",
                column: "ClientID",
                unique: true,
                filter: "[IsPrimary] = 1 AND [IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCreditProfiles_ClientID",
                table: "ClientCreditProfiles",
                column: "ClientID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AreaID",
                table: "Clients",
                column: "AreaID");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClientCode",
                table: "Clients",
                column: "ClientCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Email",
                table: "Clients",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Phone",
                table: "Clients",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_RegionID_AreaID_TerritoryID",
                table: "Clients",
                columns: new[] { "RegionID", "AreaID", "TerritoryID" });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_TerritoryID",
                table: "Clients",
                column: "TerritoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSegmentAssignments_AssignedByUserID",
                table: "ClientSegmentAssignments",
                column: "AssignedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSegmentAssignments_ClientID_ClientSegmentID",
                table: "ClientSegmentAssignments",
                columns: new[] { "ClientID", "ClientSegmentID" },
                unique: true,
                filter: "[EffectiveTo] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSegmentAssignments_ClientSegmentID",
                table: "ClientSegmentAssignments",
                column: "ClientSegmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSegments_SegmentCode",
                table: "ClientSegments",
                column: "SegmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_AssignedEmployeeID",
                table: "Complaints",
                column: "AssignedEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ClientID",
                table: "Complaints",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ComplaintNo",
                table: "Complaints",
                column: "ComplaintNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_Status_SLADueAt_AssignedEmployeeID",
                table: "Complaints",
                columns: new[] { "Status", "SLADueAt", "AssignedEmployeeID" });

            migrationBuilder.CreateIndex(
                name: "IX_CRMActivities_ClientID",
                table: "CRMActivities",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_CRMActivities_LeadID_ClientID_OpportunityID_ActivityAt",
                table: "CRMActivities",
                columns: new[] { "LeadID", "ClientID", "OpportunityID", "ActivityAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CRMActivities_OpportunityID",
                table: "CRMActivities",
                column: "OpportunityID");

            migrationBuilder.CreateIndex(
                name: "IX_CRMActivities_PerformedByEmployeeID",
                table: "CRMActivities",
                column: "PerformedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_CRMActivityParticipants_ClientContactID",
                table: "CRMActivityParticipants",
                column: "ClientContactID");

            migrationBuilder.CreateIndex(
                name: "IX_CRMActivityParticipants_CRMActivityID",
                table: "CRMActivityParticipants",
                column: "CRMActivityID");

            migrationBuilder.CreateIndex(
                name: "IX_CRMActivityParticipants_EmployeeID",
                table: "CRMActivityParticipants",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_CRMTasks_AssignedEmployeeID_Status_DueAt",
                table: "CRMTasks",
                columns: new[] { "AssignedEmployeeID", "Status", "DueAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CRMTasks_ClientID",
                table: "CRMTasks",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_CRMTasks_ComplaintID",
                table: "CRMTasks",
                column: "ComplaintID");

            migrationBuilder.CreateIndex(
                name: "IX_CRMTasks_LeadID",
                table: "CRMTasks",
                column: "LeadID");

            migrationBuilder.CreateIndex(
                name: "IX_CRMTasks_OpportunityID",
                table: "CRMTasks",
                column: "OpportunityID");

            migrationBuilder.CreateIndex(
                name: "IX_CRMTasks_ReactivationCaseID",
                table: "CRMTasks",
                column: "ReactivationCaseID");

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateReviewCases_SourceEntityType_SourceEntityID_MatchedEntityType_MatchedEntityID_Status",
                table: "DuplicateReviewCases",
                columns: new[] { "SourceEntityType", "SourceEntityID", "MatchedEntityType", "MatchedEntityID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Leads_AssignedEmployeeID",
                table: "Leads",
                column: "AssignedEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_ConvertedClientID",
                table: "Leads",
                column: "ConvertedClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Email",
                table: "Leads",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_LeadCode",
                table: "Leads",
                column: "LeadCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Phone",
                table: "Leads",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_RegionID",
                table: "Leads",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Status_AssignedEmployeeID_NextFollowUpAt",
                table: "Leads",
                columns: new[] { "Status", "AssignedEmployeeID", "NextFollowUpAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LeadScoreRules_IsActive_EffectiveFrom_EffectiveTo",
                table: "LeadScoreRules",
                columns: new[] { "IsActive", "EffectiveFrom", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_ClientID",
                table: "Opportunities",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_LeadID",
                table: "Opportunities",
                column: "LeadID");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_OpportunityCode",
                table: "Opportunities",
                column: "OpportunityCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_OwnerEmployeeID",
                table: "Opportunities",
                column: "OwnerEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_Stage_OwnerEmployeeID_ExpectedCloseDate",
                table: "Opportunities",
                columns: new[] { "Stage", "OwnerEmployeeID", "ExpectedCloseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_QuotationID_SKUID",
                table: "QuotationItems",
                columns: new[] { "QuotationID", "SKUID" });

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_ClientID_Status_ValidUntil",
                table: "Quotations",
                columns: new[] { "ClientID", "Status", "ValidUntil" });

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_OpportunityID",
                table: "Quotations",
                column: "OpportunityID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_QuotationNo_VersionNo",
                table: "Quotations",
                columns: new[] { "QuotationNo", "VersionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_RootQuotationID",
                table: "Quotations",
                column: "RootQuotationID");

            migrationBuilder.CreateIndex(
                name: "IX_ReactivationCases_AssignedEmployeeID",
                table: "ReactivationCases",
                column: "AssignedEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ReactivationCases_ClientID",
                table: "ReactivationCases",
                column: "ClientID",
                unique: true,
                filter: "[Status] <> 4 AND [Status] <> 5 AND [Status] <> 6");

            migrationBuilder.CreateIndex(
                name: "IX_ReactivationCases_Status_AssignedEmployeeID_OpenedAt",
                table: "ReactivationCases",
                columns: new[] { "Status", "AssignedEmployeeID", "OpenedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_RouteOutlets_Clients_ClientID",
                table: "RouteOutlets",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ClientID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteOutlets_Clients_ClientID",
                table: "RouteOutlets");

            migrationBuilder.DropTable(
                name: "ClientCreditProfiles");

            migrationBuilder.DropTable(
                name: "ClientSegmentAssignments");

            migrationBuilder.DropTable(
                name: "CRMActivityParticipants");

            migrationBuilder.DropTable(
                name: "CRMTasks");

            migrationBuilder.DropTable(
                name: "DuplicateReviewCases");

            migrationBuilder.DropTable(
                name: "LeadScoreRules");

            migrationBuilder.DropTable(
                name: "QuotationItems");

            migrationBuilder.DropTable(
                name: "ClientSegments");

            migrationBuilder.DropTable(
                name: "CRMActivities");

            migrationBuilder.DropTable(
                name: "ClientContacts");

            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "ReactivationCases");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropTable(
                name: "Opportunities");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_RouteOutlets_ClientID",
                table: "RouteOutlets");

            migrationBuilder.DropIndex(
                name: "IX_RouteOutlets_RouteID_ClientID",
                table: "RouteOutlets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RouteOutlets_DateRange",
                table: "RouteOutlets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RouteOutlets_VisitFrequency",
                table: "RouteOutlets");

            migrationBuilder.CreateIndex(
                name: "IX_RouteOutlets_RouteID_ClientID_EffectiveFrom",
                table: "RouteOutlets",
                columns: new[] { "RouteID", "ClientID", "EffectiveFrom" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_RouteOutlets_ClientID",
                table: "RouteOutlets",
                sql: "[ClientID] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RouteOutlets_EffectiveDates",
                table: "RouteOutlets",
                sql: "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
        }
    }
}
