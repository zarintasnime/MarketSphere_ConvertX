using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketSphere.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _01_Foundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TradeLicenseNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
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
                    table.PrimaryKey("PK_Companies", x => x.CompanyID);
                });

            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new
                {
                    DesignationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HierarchyLevel = table.Column<int>(type: "int", nullable: false),
                    IsFieldRole = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Designations", x => x.DesignationID);
                    table.CheckConstraint("CK_Designations_HierarchyLevel", "[HierarchyLevel] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    PermissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PermissionCode = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RoleLevel = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleID);
                    table.CheckConstraint("CK_Roles_RoleLevel", "[RoleLevel] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MustChangePassword = table.Column<bool>(type: "bit", nullable: false),
                    AccountActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedLoginCount = table.Column<int>(type: "int", nullable: false),
                    LockoutEndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.CheckConstraint("CK_Users_FailedLoginCount", "[FailedLoginCount] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    BranchID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BranchType = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
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
                    table.PrimaryKey("PK_Branches", x => x.BranchID);
                    table.ForeignKey(
                        name: "FK_Branches_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    RegionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    RegionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegionName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_Regions", x => x.RegionID);
                    table.ForeignKey(
                        name: "FK_Regions_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RolePermissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    PermissionID = table.Column<int>(type: "int", nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.RolePermissionID);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "Permissions",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAccountTokens",
                columns: table => new
                {
                    UserAccountTokenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    TokenType = table.Column<int>(type: "int", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccountTokens", x => x.UserAccountTokenID);
                    table.CheckConstraint("CK_UserAccountTokens_Expiry", "[ExpiresAt] > [CreatedAt]");
                    table.ForeignKey(
                        name: "FK_UserAccountTokens_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleID);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_AssignedByUserID",
                        column: x => x.AssignedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    UserSessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DeviceIdentifier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RefreshTokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.UserSessionID);
                    table.CheckConstraint("CK_UserSessions_Expiry", "[ExpiresAt] > [IssuedAt]");
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    AreaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionID = table.Column<int>(type: "int", nullable: false),
                    AreaCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AreaName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_Areas", x => x.AreaID);
                    table.ForeignKey(
                        name: "FK_Areas_Regions_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Territories",
                columns: table => new
                {
                    TerritoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaID = table.Column<int>(type: "int", nullable: false),
                    TerritoryCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TerritoryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_Territories", x => x.TerritoryID);
                    table.ForeignKey(
                        name: "FK_Territories_Areas_AreaID",
                        column: x => x.AreaID,
                        principalTable: "Areas",
                        principalColumn: "AreaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    DesignationID = table.Column<int>(type: "int", nullable: false),
                    ManagerEmployeeID = table.Column<int>(type: "int", nullable: true),
                    BranchID = table.Column<int>(type: "int", nullable: false),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    AreaID = table.Column<int>(type: "int", nullable: true),
                    TerritoryID = table.Column<int>(type: "int", nullable: true),
                    JoiningDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeID);
                    table.CheckConstraint("CK_Employees_EmploymentDates", "[EndDate] IS NULL OR [EndDate] >= [JoiningDate]");
                    table.ForeignKey(
                        name: "FK_Employees_Areas_AreaID",
                        column: x => x.AreaID,
                        principalTable: "Areas",
                        principalColumn: "AreaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Designations_DesignationID",
                        column: x => x.DesignationID,
                        principalTable: "Designations",
                        principalColumn: "DesignationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_ManagerEmployeeID",
                        column: x => x.ManagerEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Regions_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Territories_TerritoryID",
                        column: x => x.TerritoryID,
                        principalTable: "Territories",
                        principalColumn: "TerritoryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RouteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TerritoryID = table.Column<int>(type: "int", nullable: false),
                    RouteCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RouteName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: true),
                    VisitFrequency = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Routes", x => x.RouteID);
                    table.ForeignKey(
                        name: "FK_Routes_Territories_TerritoryID",
                        column: x => x.TerritoryID,
                        principalTable: "Territories",
                        principalColumn: "TerritoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTerritoryAssignments",
                columns: table => new
                {
                    EmployeeTerritoryAssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    ScopeType = table.Column<int>(type: "int", nullable: false),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    AreaID = table.Column<int>(type: "int", nullable: true),
                    TerritoryID = table.Column<int>(type: "int", nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTerritoryAssignments", x => x.EmployeeTerritoryAssignmentID);
                    table.CheckConstraint("CK_EmployeeTerritoryAssignments_EffectiveDates", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.CheckConstraint("CK_EmployeeTerritoryAssignments_Scope", "([ScopeType] = 1 AND [RegionID] IS NOT NULL AND [AreaID] IS NULL AND [TerritoryID] IS NULL) OR ([ScopeType] = 2 AND [RegionID] IS NULL AND [AreaID] IS NOT NULL AND [TerritoryID] IS NULL) OR ([ScopeType] = 3 AND [RegionID] IS NULL AND [AreaID] IS NULL AND [TerritoryID] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_EmployeeTerritoryAssignments_Areas_AreaID",
                        column: x => x.AreaID,
                        principalTable: "Areas",
                        principalColumn: "AreaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTerritoryAssignments_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTerritoryAssignments_Regions_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTerritoryAssignments_Territories_TerritoryID",
                        column: x => x.TerritoryID,
                        principalTable: "Territories",
                        principalColumn: "TerritoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRouteAssignments",
                columns: table => new
                {
                    EmployeeRouteAssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    RouteID = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    DayOfWeek = table.Column<int>(type: "int", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRouteAssignments", x => x.EmployeeRouteAssignmentID);
                    table.CheckConstraint("CK_EmployeeRouteAssignments_EffectiveDates", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.ForeignKey(
                        name: "FK_EmployeeRouteAssignments_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeRouteAssignments_Routes_RouteID",
                        column: x => x.RouteID,
                        principalTable: "Routes",
                        principalColumn: "RouteID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RouteOutlets",
                columns: table => new
                {
                    RouteOutletID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteID = table.Column<int>(type: "int", nullable: false),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    VisitFrequency = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteOutlets", x => x.RouteOutletID);
                    table.CheckConstraint("CK_RouteOutlets_ClientID", "[ClientID] > 0");
                    table.CheckConstraint("CK_RouteOutlets_EffectiveDates", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.CheckConstraint("CK_RouteOutlets_SequenceNo", "[SequenceNo] > 0");
                    table.ForeignKey(
                        name: "FK_RouteOutlets_Routes_RouteID",
                        column: x => x.RouteID,
                        principalTable: "Routes",
                        principalColumn: "RouteID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_RegionID_AreaCode",
                table: "Areas",
                columns: new[] { "RegionID", "AreaCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CompanyID_BranchCode",
                table: "Branches",
                columns: new[] { "CompanyID", "BranchCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CompanyCode",
                table: "Companies",
                column: "CompanyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Designations_DesignationCode",
                table: "Designations",
                column: "DesignationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRouteAssignments_EmployeeID_IsPrimary_Status_EffectiveFrom",
                table: "EmployeeRouteAssignments",
                columns: new[] { "EmployeeID", "IsPrimary", "Status", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRouteAssignments_EmployeeID_RouteID_EffectiveFrom",
                table: "EmployeeRouteAssignments",
                columns: new[] { "EmployeeID", "RouteID", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRouteAssignments_RouteID",
                table: "EmployeeRouteAssignments",
                column: "RouteID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_AreaID",
                table: "Employees",
                column: "AreaID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BranchID_Status",
                table: "Employees",
                columns: new[] { "BranchID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DesignationID",
                table: "Employees",
                column: "DesignationID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerEmployeeID",
                table: "Employees",
                column: "ManagerEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_RegionID",
                table: "Employees",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TerritoryID",
                table: "Employees",
                column: "TerritoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserID",
                table: "Employees",
                column: "UserID",
                unique: true,
                filter: "[UserID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTerritoryAssignments_AreaID",
                table: "EmployeeTerritoryAssignments",
                column: "AreaID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTerritoryAssignments_EmployeeID_ScopeType_RegionID_AreaID_TerritoryID_EffectiveFrom",
                table: "EmployeeTerritoryAssignments",
                columns: new[] { "EmployeeID", "ScopeType", "RegionID", "AreaID", "TerritoryID", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTerritoryAssignments_RegionID",
                table: "EmployeeTerritoryAssignments",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTerritoryAssignments_TerritoryID",
                table: "EmployeeTerritoryAssignments",
                column: "TerritoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ModuleName_ActionName",
                table: "Permissions",
                columns: new[] { "ModuleName", "ActionName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermissionCode",
                table: "Permissions",
                column: "PermissionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_CompanyID_RegionCode",
                table: "Regions",
                columns: new[] { "CompanyID", "RegionCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionID",
                table: "RolePermissions",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleID_PermissionID",
                table: "RolePermissions",
                columns: new[] { "RoleID", "PermissionID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleCode",
                table: "Roles",
                column: "RoleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteOutlets_RouteID_ClientID_EffectiveFrom",
                table: "RouteOutlets",
                columns: new[] { "RouteID", "ClientID", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_RouteOutlets_RouteID_SequenceNo_EffectiveFrom",
                table: "RouteOutlets",
                columns: new[] { "RouteID", "SequenceNo", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_Routes_TerritoryID_RouteCode",
                table: "Routes",
                columns: new[] { "TerritoryID", "RouteCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Territories_AreaID_TerritoryCode",
                table: "Territories",
                columns: new[] { "AreaID", "TerritoryCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountTokens_TokenHash",
                table: "UserAccountTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountTokens_UserID_TokenType_ExpiresAt",
                table: "UserAccountTokens",
                columns: new[] { "UserID", "TokenType", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AssignedByUserID",
                table: "UserRoles",
                column: "AssignedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleID",
                table: "UserRoles",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserID_RoleID",
                table: "UserRoles",
                columns: new[] { "UserID", "RoleID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status",
                table: "Users",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_RefreshTokenHash",
                table: "UserSessions",
                column: "RefreshTokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserID_RevokedAt_ExpiresAt",
                table: "UserSessions",
                columns: new[] { "UserID", "RevokedAt", "ExpiresAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeRouteAssignments");

            migrationBuilder.DropTable(
                name: "EmployeeTerritoryAssignments");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "RouteOutlets");

            migrationBuilder.DropTable(
                name: "UserAccountTokens");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Designations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Territories");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
