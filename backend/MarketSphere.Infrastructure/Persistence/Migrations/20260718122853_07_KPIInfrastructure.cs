using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _07_KPIInfrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalPolicies",
                columns: table => new
                {
                    ApprovalPolicyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalType = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BranchID = table.Column<int>(type: "int", nullable: true),
                    Channel = table.Column<int>(type: "int", nullable: true),
                    MinimumAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaximumAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MinimumDiscountPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    MaximumDiscountPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalPolicies", x => x.ApprovalPolicyID);
                    table.CheckConstraint("CK_ApprovalPolicies_AmountRange", "[MaximumAmount] IS NULL OR [MinimumAmount] IS NULL OR [MaximumAmount] >= [MinimumAmount]");
                    table.CheckConstraint("CK_ApprovalPolicies_DiscountRange", "[MaximumDiscountPercent] IS NULL OR [MinimumDiscountPercent] IS NULL OR [MaximumDiscountPercent] >= [MinimumDiscountPercent]");
                    table.CheckConstraint("CK_ApprovalPolicies_Period", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.ForeignKey(
                        name: "FK_ApprovalPolicies_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityID = table.Column<int>(type: "int", nullable: true),
                    OldValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeviceIdentifier = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogID);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTargets",
                columns: table => new
                {
                    EmployeeTargetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    TargetPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    TargetValue = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CampaignID = table.Column<int>(type: "int", nullable: true),
                    SKUID = table.Column<int>(type: "int", nullable: true),
                    ClientID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTargets", x => x.EmployeeTargetID);
                    table.CheckConstraint("CK_EmployeeTargets_Period", "[TargetPeriodEnd] >= [TargetPeriodStart]");
                    table.CheckConstraint("CK_EmployeeTargets_Value", "[TargetValue] > 0 AND ([TargetAmount] IS NULL OR [TargetAmount] >= 0)");
                    table.ForeignKey(
                        name: "FK_EmployeeTargets_Campaigns_CampaignID",
                        column: x => x.CampaignID,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTargets_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTargets_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTargets_SKUs_SKUID",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "SKUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachments",
                columns: table => new
                {
                    FileAttachmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReferenceID = table.Column<int>(type: "int", nullable: false),
                    AttachmentCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsEvidence = table.Column<bool>(type: "bit", nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GPS = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    VerifiedByUserID = table.Column<int>(type: "int", nullable: true),
                    UploadedByUserID = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachments", x => x.FileAttachmentID);
                    table.CheckConstraint("CK_FileAttachments_Size", "[FileSizeBytes] > 0");
                    table.ForeignKey(
                        name: "FK_FileAttachments_Users_UploadedByUserID",
                        column: x => x.UploadedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileAttachments_Users_VerifiedByUserID",
                        column: x => x.VerifiedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdempotencyRequests",
                columns: table => new
                {
                    IdempotencyRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RequestHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ResponseStatusCode = table.Column<int>(type: "int", nullable: true),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyRequests", x => x.IdempotencyRequestID);
                    table.ForeignKey(
                        name: "FK_IdempotencyRequests_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReferenceID = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NumberSequences",
                columns: table => new
                {
                    NumberSequenceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    YearValue = table.Column<int>(type: "int", nullable: true),
                    BranchID = table.Column<int>(type: "int", nullable: true),
                    LastNumber = table.Column<long>(type: "bigint", nullable: false),
                    PaddingLength = table.Column<int>(type: "int", nullable: false),
                    ResetPolicy = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberSequences", x => x.NumberSequenceID);
                    table.CheckConstraint("CK_NumberSequences_LastNumber", "[LastNumber] >= 0");
                    table.CheckConstraint("CK_NumberSequences_Padding", "[PaddingLength] BETWEEN 1 AND 20");
                    table.ForeignKey(
                        name: "FK_NumberSequences_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfflineSyncRecords",
                columns: table => new
                {
                    OfflineSyncRecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserSessionID = table.Column<int>(type: "int", nullable: false),
                    LocalRecordKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServerTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncStatus = table.Column<int>(type: "int", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ServerEntityID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfflineSyncRecords", x => x.OfflineSyncRecordID);
                    table.CheckConstraint("CK_OfflineSyncRecords_RetryCount", "[RetryCount] >= 0");
                    table.ForeignKey(
                        name: "FK_OfflineSyncRecords_UserSessions_UserSessionID",
                        column: x => x.UserSessionID,
                        principalTable: "UserSessions",
                        principalColumn: "UserSessionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RewardRules",
                columns: table => new
                {
                    RewardRuleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ApplicableDesignationID = table.Column<int>(type: "int", nullable: true),
                    RewardType = table.Column<int>(type: "int", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    MinimumAchievementPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    MaximumAchievementPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    CalculationType = table.Column<int>(type: "int", nullable: false),
                    FixedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RatePercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    MaximumCap = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardRules", x => x.RewardRuleID);
                    table.CheckConstraint("CK_RewardRules_Achievement", "[MinimumAchievementPercent] >= 0 AND ([MaximumAchievementPercent] IS NULL OR [MaximumAchievementPercent] >= [MinimumAchievementPercent])");
                    table.CheckConstraint("CK_RewardRules_Amounts", "([FixedAmount] IS NULL OR [FixedAmount] >= 0) AND ([RatePercent] IS NULL OR [RatePercent] >= 0) AND ([MaximumCap] IS NULL OR [MaximumCap] >= 0)");
                    table.CheckConstraint("CK_RewardRules_Period", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.ForeignKey(
                        name: "FK_RewardRules_Designations_ApplicableDesignationID",
                        column: x => x.ApplicableDesignationID,
                        principalTable: "Designations",
                        principalColumn: "DesignationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatusHistories",
                columns: table => new
                {
                    StatusHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityID = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NewStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedByUserID = table.Column<int>(type: "int", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusHistories", x => x.StatusHistoryID);
                    table.ForeignKey(
                        name: "FK_StatusHistories_Users_ChangedByUserID",
                        column: x => x.ChangedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    SystemSettingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettingKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SettingValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    ScopeType = table.Column<int>(type: "int", nullable: false),
                    ScopeID = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.SystemSettingID);
                    table.ForeignKey(
                        name: "FK_SystemSettings_Users_UpdatedByUserID",
                        column: x => x.UpdatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRequests",
                columns: table => new
                {
                    ApprovalRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReferenceID = table.Column<int>(type: "int", nullable: false),
                    ApprovalType = table.Column<int>(type: "int", nullable: false),
                    ApprovalPolicyID = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserID = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStepNo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequests", x => x.ApprovalRequestID);
                    table.CheckConstraint("CK_ApprovalRequests_CurrentStep", "[CurrentStepNo] > 0");
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_ApprovalPolicies_ApprovalPolicyID",
                        column: x => x.ApprovalPolicyID,
                        principalTable: "ApprovalPolicies",
                        principalColumn: "ApprovalPolicyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_Users_RequestedByUserID",
                        column: x => x.RequestedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalStepDefinitions",
                columns: table => new
                {
                    ApprovalStepDefinitionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalPolicyID = table.Column<int>(type: "int", nullable: false),
                    StepNo = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ApprovalMode = table.Column<int>(type: "int", nullable: false),
                    MinimumApprovals = table.Column<int>(type: "int", nullable: false),
                    IsFinalStep = table.Column<bool>(type: "bit", nullable: false),
                    EscalationHours = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStepDefinitions", x => x.ApprovalStepDefinitionID);
                    table.CheckConstraint("CK_ApprovalStepDefinitions_Values", "[StepNo] > 0 AND [MinimumApprovals] > 0 AND ([EscalationHours] IS NULL OR [EscalationHours] > 0)");
                    table.ForeignKey(
                        name: "FK_ApprovalStepDefinitions_ApprovalPolicies_ApprovalPolicyID",
                        column: x => x.ApprovalPolicyID,
                        principalTable: "ApprovalPolicies",
                        principalColumn: "ApprovalPolicyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RewardCalculations",
                columns: table => new
                {
                    RewardCalculationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeTargetID = table.Column<int>(type: "int", nullable: true),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    RewardRuleID = table.Column<int>(type: "int", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualValue = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    AchievementPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    EligibleBaseAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RewardAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AdjustmentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardCalculations", x => x.RewardCalculationID);
                    table.CheckConstraint("CK_RewardCalculations_Amounts", "[ActualValue] >= 0 AND [AchievementPercent] >= 0 AND [EligibleBaseAmount] >= 0 AND [RewardAmount] >= 0 AND [FinalAmount] >= 0");
                    table.CheckConstraint("CK_RewardCalculations_Period", "[PeriodEnd] >= [PeriodStart]");
                    table.ForeignKey(
                        name: "FK_RewardCalculations_EmployeeTargets_EmployeeTargetID",
                        column: x => x.EmployeeTargetID,
                        principalTable: "EmployeeTargets",
                        principalColumn: "EmployeeTargetID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RewardCalculations_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RewardCalculations_RewardRules_RewardRuleID",
                        column: x => x.RewardRuleID,
                        principalTable: "RewardRules",
                        principalColumn: "RewardRuleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalActions",
                columns: table => new
                {
                    ApprovalActionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalRequestID = table.Column<int>(type: "int", nullable: false),
                    StepNo = table.Column<int>(type: "int", nullable: false),
                    ActionByUserID = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DelegatedFromUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalActions", x => x.ApprovalActionID);
                    table.ForeignKey(
                        name: "FK_ApprovalActions_ApprovalRequests_ApprovalRequestID",
                        column: x => x.ApprovalRequestID,
                        principalTable: "ApprovalRequests",
                        principalColumn: "ApprovalRequestID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalActions_Users_ActionByUserID",
                        column: x => x.ActionByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalActions_Users_DelegatedFromUserID",
                        column: x => x.DelegatedFromUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalStepAssignees",
                columns: table => new
                {
                    ApprovalStepAssigneeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalStepDefinitionID = table.Column<int>(type: "int", nullable: false),
                    AssigneeType = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: true),
                    DesignationID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    EmployeeID = table.Column<int>(type: "int", nullable: true),
                    IsFallback = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStepAssignees", x => x.ApprovalStepAssigneeID);
                    table.CheckConstraint("CK_ApprovalStepAssignees_OneReference", "(CASE WHEN [RoleID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [DesignationID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [UserID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [EmployeeID] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.ForeignKey(
                        name: "FK_ApprovalStepAssignees_ApprovalStepDefinitions_ApprovalStepDefinitionID",
                        column: x => x.ApprovalStepDefinitionID,
                        principalTable: "ApprovalStepDefinitions",
                        principalColumn: "ApprovalStepDefinitionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalStepAssignees_Designations_DesignationID",
                        column: x => x.DesignationID,
                        principalTable: "Designations",
                        principalColumn: "DesignationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalStepAssignees_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalStepAssignees_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalStepAssignees_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ApprovalRequestID",
                table: "StockTransfers",
                column: "ApprovalRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ProofFileAttachmentID",
                table: "Payments",
                column: "ProofFileAttachmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApprovalRequestID",
                table: "Orders",
                column: "ApprovalRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalActions_ActionByUserID",
                table: "ApprovalActions",
                column: "ActionByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalActions_ApprovalRequestID_StepNo_ActionAt",
                table: "ApprovalActions",
                columns: new[] { "ApprovalRequestID", "StepNo", "ActionAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalActions_DelegatedFromUserID",
                table: "ApprovalActions",
                column: "DelegatedFromUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalPolicies_ApprovalType_EntityType_IsActive_Priority",
                table: "ApprovalPolicies",
                columns: new[] { "ApprovalType", "EntityType", "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalPolicies_BranchID",
                table: "ApprovalPolicies",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_ApprovalPolicyID",
                table: "ApprovalRequests",
                column: "ApprovalPolicyID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_ReferenceType_ReferenceID_ApprovalType",
                table: "ApprovalRequests",
                columns: new[] { "ReferenceType", "ReferenceID", "ApprovalType" },
                unique: true,
                filter: "[Status] IN (1,2)");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_RequestedByUserID",
                table: "ApprovalRequests",
                column: "RequestedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_Status_CurrentStepNo_RequestedAt",
                table: "ApprovalRequests",
                columns: new[] { "Status", "CurrentStepNo", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepAssignees_ApprovalStepDefinitionID_AssigneeType_RoleID_DesignationID_UserID_EmployeeID",
                table: "ApprovalStepAssignees",
                columns: new[] { "ApprovalStepDefinitionID", "AssigneeType", "RoleID", "DesignationID", "UserID", "EmployeeID" },
                unique: true,
                filter: "[RoleID] IS NOT NULL AND [DesignationID] IS NOT NULL AND [UserID] IS NOT NULL AND [EmployeeID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepAssignees_DesignationID",
                table: "ApprovalStepAssignees",
                column: "DesignationID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepAssignees_EmployeeID",
                table: "ApprovalStepAssignees",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepAssignees_RoleID",
                table: "ApprovalStepAssignees",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepAssignees_UserID",
                table: "ApprovalStepAssignees",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepDefinitions_ApprovalPolicyID_StepNo",
                table: "ApprovalStepDefinitions",
                columns: new[] { "ApprovalPolicyID", "StepNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityID_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserID_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "UserID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTargets_CampaignID",
                table: "EmployeeTargets",
                column: "CampaignID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTargets_ClientID",
                table: "EmployeeTargets",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTargets_EmployeeID_TargetPeriodStart_TargetPeriodEnd_TargetType_CampaignID_SKUID_ClientID",
                table: "EmployeeTargets",
                columns: new[] { "EmployeeID", "TargetPeriodStart", "TargetPeriodEnd", "TargetType", "CampaignID", "SKUID", "ClientID" },
                unique: true,
                filter: "[CampaignID] IS NOT NULL AND [SKUID] IS NOT NULL AND [ClientID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTargets_SKUID",
                table: "EmployeeTargets",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTargets_Status_TargetPeriodEnd",
                table: "EmployeeTargets",
                columns: new[] { "Status", "TargetPeriodEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_FileHash",
                table: "FileAttachments",
                column: "FileHash");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_ReferenceType_ReferenceID_AttachmentCategory",
                table: "FileAttachments",
                columns: new[] { "ReferenceType", "ReferenceID", "AttachmentCategory" });

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_StoredFileName",
                table: "FileAttachments",
                column: "StoredFileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_UploadedByUserID",
                table: "FileAttachments",
                column: "UploadedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_VerifiedByUserID",
                table: "FileAttachments",
                column: "VerifiedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRequests_ExpiresAt",
                table: "IdempotencyRequests",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRequests_IdempotencyKey",
                table: "IdempotencyRequests",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRequests_UserID",
                table: "IdempotencyRequests",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReferenceType_ReferenceID_Title",
                table: "Notifications",
                columns: new[] { "ReferenceType", "ReferenceID", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserID_IsRead_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserID", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_NumberSequences_BranchID",
                table: "NumberSequences",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_NumberSequences_DocumentType_YearValue_BranchID",
                table: "NumberSequences",
                columns: new[] { "DocumentType", "YearValue", "BranchID" },
                unique: true,
                filter: "[YearValue] IS NOT NULL AND [BranchID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OfflineSyncRecords_SyncStatus_RetryCount",
                table: "OfflineSyncRecords",
                columns: new[] { "SyncStatus", "RetryCount" });

            migrationBuilder.CreateIndex(
                name: "IX_OfflineSyncRecords_UserSessionID_LocalRecordKey",
                table: "OfflineSyncRecords",
                columns: new[] { "UserSessionID", "LocalRecordKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RewardCalculations_EmployeeID_PeriodStart_PeriodEnd_Status",
                table: "RewardCalculations",
                columns: new[] { "EmployeeID", "PeriodStart", "PeriodEnd", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RewardCalculations_EmployeeTargetID",
                table: "RewardCalculations",
                column: "EmployeeTargetID",
                unique: true,
                filter: "[EmployeeTargetID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RewardCalculations_RewardRuleID",
                table: "RewardCalculations",
                column: "RewardRuleID");

            migrationBuilder.CreateIndex(
                name: "IX_RewardRules_ApplicableDesignationID",
                table: "RewardRules",
                column: "ApplicableDesignationID");

            migrationBuilder.CreateIndex(
                name: "IX_RewardRules_TargetType_ApplicableDesignationID_IsActive_EffectiveFrom",
                table: "RewardRules",
                columns: new[] { "TargetType", "ApplicableDesignationID", "IsActive", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_StatusHistories_ChangedByUserID",
                table: "StatusHistories",
                column: "ChangedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_StatusHistories_EntityType_EntityID_ChangedAt",
                table: "StatusHistories",
                columns: new[] { "EntityType", "EntityID", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_SettingKey_ScopeType_ScopeID",
                table: "SystemSettings",
                columns: new[] { "SettingKey", "ScopeType", "ScopeID" },
                unique: true,
                filter: "[ScopeID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_UpdatedByUserID",
                table: "SystemSettings",
                column: "UpdatedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ApprovalRequests_ApprovalRequestID",
                table: "Orders",
                column: "ApprovalRequestID",
                principalTable: "ApprovalRequests",
                principalColumn: "ApprovalRequestID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_FileAttachments_ProofFileAttachmentID",
                table: "Payments",
                column: "ProofFileAttachmentID",
                principalTable: "FileAttachments",
                principalColumn: "FileAttachmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_ApprovalRequests_ApprovalRequestID",
                table: "StockTransfers",
                column: "ApprovalRequestID",
                principalTable: "ApprovalRequests",
                principalColumn: "ApprovalRequestID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ApprovalRequests_ApprovalRequestID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_FileAttachments_ProofFileAttachmentID",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_ApprovalRequests_ApprovalRequestID",
                table: "StockTransfers");

            migrationBuilder.DropTable(
                name: "ApprovalActions");

            migrationBuilder.DropTable(
                name: "ApprovalStepAssignees");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "FileAttachments");

            migrationBuilder.DropTable(
                name: "IdempotencyRequests");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NumberSequences");

            migrationBuilder.DropTable(
                name: "OfflineSyncRecords");

            migrationBuilder.DropTable(
                name: "RewardCalculations");

            migrationBuilder.DropTable(
                name: "StatusHistories");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "ApprovalRequests");

            migrationBuilder.DropTable(
                name: "ApprovalStepDefinitions");

            migrationBuilder.DropTable(
                name: "EmployeeTargets");

            migrationBuilder.DropTable(
                name: "RewardRules");

            migrationBuilder.DropTable(
                name: "ApprovalPolicies");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_ApprovalRequestID",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ProofFileAttachmentID",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ApprovalRequestID",
                table: "Orders");
        }
    }
}
