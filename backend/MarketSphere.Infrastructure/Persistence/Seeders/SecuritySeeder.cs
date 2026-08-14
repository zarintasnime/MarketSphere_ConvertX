using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Infrastructure.Persistence.Seeders;

public sealed class SecuritySeeder
{
    private readonly MarketSphereDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IDateTimeProvider _clock;

    public SecuritySeeder(
        MarketSphereDbContext db,
        IConfiguration configuration,
        IPasswordHashService passwordHashService,
        IDateTimeProvider clock)
    {
        _db = db;
        _configuration = configuration;
        _passwordHashService = passwordHashService;
        _clock = clock;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        await SeedRolesAsync(cancellationToken);
        await SeedPermissionsAsync(cancellationToken);
        await SeedSuperAdminAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(
        CancellationToken cancellationToken)
    {
        var roles = new[]
        {
            new RoleSeed(RoleCodes.SuperAdmin, "Super Admin", 0),
            new RoleSeed(RoleCodes.Admin, "Admin", 10),
            new RoleSeed(RoleCodes.GeneralManager, "General Manager", 20),
            new RoleSeed(RoleCodes.CrmManager, "CRM Manager", 30),
            new RoleSeed(RoleCodes.MarketingManager, "Marketing Manager", 30),
            new RoleSeed(RoleCodes.SalesManager, "Sales Manager", 30),
            new RoleSeed(RoleCodes.SalesOfficer, "Sales Officer", 50),
            new RoleSeed(RoleCodes.ModernTradeExecutive, "MT Executive", 50),
            new RoleSeed(RoleCodes.BusinessPromoter, "Business Promoter", 60),
            new RoleSeed(RoleCodes.Merchandiser, "Merchandiser", 60),
            new RoleSeed(RoleCodes.ProcurementOfficer, "Procurement Officer", 50),
            new RoleSeed(RoleCodes.WarehouseOfficer, "Warehouse Officer", 50),
            new RoleSeed(RoleCodes.DeliveryOfficer, "Delivery Officer", 60)
        };

        foreach (var seed in roles)
        {
            var role = await _db.Roles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x => x.RoleCode == seed.Code,
                    cancellationToken);

            if (role is null)
            {
                role = new Role
                {
                    RoleCode = seed.Code,
                    RoleName = seed.Name,
                    RoleLevel = seed.Level,
                    Description = $"{seed.Name} system role.",
                    IsActive = true,
                    CreatedAt = _clock.UtcNow
                };

                await _db.Roles.AddAsync(
                    role,
                    cancellationToken);
            }
            else
            {
                role.RoleName = seed.Name;
                role.RoleLevel = seed.Level;
                role.IsActive = true;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedPermissionsAsync(
        CancellationToken cancellationToken)
    {
        var permissions = new[]
        {
            new PermissionSeed(PermissionCodes.UsersView, "users", "view"),
            new PermissionSeed(PermissionCodes.UsersCreate, "users", "create"),
            new PermissionSeed(PermissionCodes.UsersUpdate, "users", "update"),
            new PermissionSeed(PermissionCodes.UsersChangeStatus, "users", "change_status"),
            new PermissionSeed(PermissionCodes.UsersAssignRoles, "users", "assign_roles"),
            new PermissionSeed(PermissionCodes.UsersCreateToken, "users", "create_token"),
            new PermissionSeed(PermissionCodes.RolesView, "roles", "view"),
            new PermissionSeed(PermissionCodes.RolesCreate, "roles", "create"),
            new PermissionSeed(PermissionCodes.RolesUpdate, "roles", "update"),
            new PermissionSeed(PermissionCodes.RolesManagePermissions, "roles", "manage_permissions"),
            new PermissionSeed(PermissionCodes.EmployeesView, "employees", "view"),
            new PermissionSeed(PermissionCodes.EmployeesCreate, "employees", "create"),
            new PermissionSeed(PermissionCodes.EmployeesUpdate, "employees", "update"),
            new PermissionSeed(PermissionCodes.OrganizationView, "organization", "view"),
            new PermissionSeed(PermissionCodes.OrganizationManage, "organization", "manage"),
            new PermissionSeed(PermissionCodes.GeographyView, "geography", "view"),
            new PermissionSeed(PermissionCodes.GeographyManage, "geography", "manage"),
            new PermissionSeed(PermissionCodes.RoutesView, "routes", "view"),
            new PermissionSeed(PermissionCodes.RoutesManage, "routes", "manage"),
            new PermissionSeed(PermissionCodes.AssignmentsManage, "assignments", "manage"),
            new PermissionSeed(PermissionCodes.ClientsView, "crm_clients", "view"),
            new PermissionSeed(PermissionCodes.ClientsManage, "crm_clients", "manage"),
            new PermissionSeed(PermissionCodes.ClientCreditManage, "crm_client_credit", "manage"),
            new PermissionSeed(PermissionCodes.LeadsView, "crm_leads", "view"),
            new PermissionSeed(PermissionCodes.LeadsManage, "crm_leads", "manage"),
            new PermissionSeed(PermissionCodes.LeadScoreRulesManage, "crm_lead_score_rules", "manage"),
            new PermissionSeed(PermissionCodes.DuplicateReviewsManage, "crm_duplicate_reviews", "manage"),
            new PermissionSeed(PermissionCodes.ActivitiesView, "crm_activities", "view"),
            new PermissionSeed(PermissionCodes.ActivitiesManage, "crm_activities", "manage"),
            new PermissionSeed(PermissionCodes.TasksView, "crm_tasks", "view"),
            new PermissionSeed(PermissionCodes.TasksManage, "crm_tasks", "manage"),
            new PermissionSeed(PermissionCodes.OpportunitiesView, "crm_opportunities", "view"),
            new PermissionSeed(PermissionCodes.OpportunitiesManage, "crm_opportunities", "manage"),
            new PermissionSeed(PermissionCodes.QuotationsView, "crm_quotations", "view"),
            new PermissionSeed(PermissionCodes.QuotationsManage, "crm_quotations", "manage"),
            new PermissionSeed(PermissionCodes.QuotationsApprove, "crm_quotations", "approve"),
            new PermissionSeed(PermissionCodes.ComplaintsView, "crm_complaints", "view"),
            new PermissionSeed(PermissionCodes.ComplaintsManage, "crm_complaints", "manage"),
            new PermissionSeed(PermissionCodes.ReactivationView, "crm_reactivation", "view"),
            new PermissionSeed(PermissionCodes.ReactivationManage, "crm_reactivation", "manage"),
            new PermissionSeed(PermissionCodes.CrmDashboardView, "crm_dashboard", "view"),
            new PermissionSeed(PermissionCodes.CampaignsView, "marketing_campaigns", "view"),
            new PermissionSeed(PermissionCodes.CampaignsManage, "marketing_campaigns", "manage"),
            new PermissionSeed(PermissionCodes.CampaignsApprove, "marketing_campaigns", "approve"),
            new PermissionSeed(PermissionCodes.CampaignExpensesManage, "marketing_campaign_expenses", "manage"),
            new PermissionSeed(PermissionCodes.CampaignAttributionManage, "marketing_campaign_attribution", "manage"),
            new PermissionSeed(PermissionCodes.VisitsView, "marketing_visits", "view"),
            new PermissionSeed(PermissionCodes.VisitsManage, "marketing_visits", "manage"),
            new PermissionSeed(PermissionCodes.SamplingView, "marketing_sampling", "view"),
            new PermissionSeed(PermissionCodes.SamplingManage, "marketing_sampling", "manage"),
            new PermissionSeed(PermissionCodes.FeedbackView, "marketing_feedback", "view"),
            new PermissionSeed(PermissionCodes.FeedbackManage, "marketing_feedback", "manage"),
            new PermissionSeed(PermissionCodes.MarketObservationsView, "marketing_market_observations", "view"),
            new PermissionSeed(PermissionCodes.MarketObservationsManage, "marketing_market_observations", "manage"),
            new PermissionSeed(PermissionCodes.BpSellOutView, "marketing_bp_sell_out", "view"),
            new PermissionSeed(PermissionCodes.BpSellOutManage, "marketing_bp_sell_out", "manage"),
            new PermissionSeed(PermissionCodes.BpSellOutVerify, "marketing_bp_sell_out", "verify"),
            new PermissionSeed(PermissionCodes.ProductCategoriesView, "product_categories", "view"),
            new PermissionSeed(PermissionCodes.ProductCategoriesManage, "product_categories", "manage"),
            new PermissionSeed(PermissionCodes.BrandsView, "brands", "view"),
            new PermissionSeed(PermissionCodes.BrandsManage, "brands", "manage"),
            new PermissionSeed(PermissionCodes.ProductsView, "products", "view"),
            new PermissionSeed(PermissionCodes.ProductsManage, "products", "manage"),
            new PermissionSeed(PermissionCodes.SKUsView, "skus", "view"),
            new PermissionSeed(PermissionCodes.SKUsManage, "skus", "manage"),
            new PermissionSeed(PermissionCodes.PriceListsView, "price_lists", "view"),
            new PermissionSeed(PermissionCodes.PriceListsManage, "price_lists", "manage"),
            new PermissionSeed(PermissionCodes.DiscountRulesView, "discount_rules", "view"),
            new PermissionSeed(PermissionCodes.DiscountRulesManage, "discount_rules", "manage"),
            new PermissionSeed(PermissionCodes.SuppliersView, "procurement_suppliers", "view"),
            new PermissionSeed(PermissionCodes.SuppliersManage, "procurement_suppliers", "manage"),
            new PermissionSeed(PermissionCodes.PurchaseRequisitionsView, "purchase_requisitions", "view"),
            new PermissionSeed(PermissionCodes.PurchaseRequisitionsManage, "purchase_requisitions", "manage"),
            new PermissionSeed(PermissionCodes.PurchaseRequisitionsApprove, "purchase_requisitions", "approve"),
            new PermissionSeed(PermissionCodes.PurchaseOrdersView, "purchase_orders", "view"),
            new PermissionSeed(PermissionCodes.PurchaseOrdersManage, "purchase_orders", "manage"),
            new PermissionSeed(PermissionCodes.PurchaseOrdersApprove, "purchase_orders", "approve"),
            new PermissionSeed(PermissionCodes.GoodsReceiptsView, "goods_receipts", "view"),
            new PermissionSeed(PermissionCodes.GoodsReceiptsManage, "goods_receipts", "manage"),
            new PermissionSeed(PermissionCodes.GoodsReceiptsPost, "goods_receipts", "post"),
            new PermissionSeed(PermissionCodes.PurchaseInvoicesView, "purchase_invoices", "view"),
            new PermissionSeed(PermissionCodes.PurchaseInvoicesManage, "purchase_invoices", "manage"),
            new PermissionSeed(PermissionCodes.SupplierPaymentsManage, "supplier_payments", "manage"),
            new PermissionSeed(PermissionCodes.SupplierReturnsView, "supplier_returns", "view"),
            new PermissionSeed(PermissionCodes.SupplierReturnsManage, "supplier_returns", "manage"),
            new PermissionSeed(PermissionCodes.SupplierReturnsPost, "supplier_returns", "post"),
            new PermissionSeed(PermissionCodes.WarehousesView, "warehouses", "view"),
            new PermissionSeed(PermissionCodes.WarehousesManage, "warehouses", "manage"),
            new PermissionSeed(PermissionCodes.StockView, "stock", "view"),
            new PermissionSeed(PermissionCodes.StockMovementsView, "stock_movements", "view"),
            new PermissionSeed(PermissionCodes.StockTransfersView, "stock_transfers", "view"),
            new PermissionSeed(PermissionCodes.StockTransfersManage, "stock_transfers", "manage"),
            new PermissionSeed(PermissionCodes.StockTransfersApprove, "stock_transfers", "approve"),
            new PermissionSeed(PermissionCodes.StockAdjustmentsView, "stock_adjustments", "view"),
            new PermissionSeed(PermissionCodes.StockAdjustmentsManage, "stock_adjustments", "manage"),
            new PermissionSeed(PermissionCodes.StockAdjustmentsApprove, "stock_adjustments", "approve"),

            new PermissionSeed(PermissionCodes.ModernTradePurchaseOrdersView, "mt_purchase_orders", "view"),
            new PermissionSeed(PermissionCodes.ModernTradePurchaseOrdersManage, "mt_purchase_orders", "manage"),
            new PermissionSeed(PermissionCodes.ModernTradePurchaseOrdersVerify, "mt_purchase_orders", "verify"),
            new PermissionSeed(PermissionCodes.OrdersView, "orders", "view"),
            new PermissionSeed(PermissionCodes.OrdersManage, "orders", "manage"),
            new PermissionSeed(PermissionCodes.OrdersSubmit, "orders", "submit"),
            new PermissionSeed(PermissionCodes.OrdersApprove, "orders", "approve"),
            new PermissionSeed(PermissionCodes.OrdersReserveStock, "orders", "reserve_stock"),
            new PermissionSeed(PermissionCodes.AppliedOffersManage, "applied_offers", "manage"),
            new PermissionSeed(PermissionCodes.InvoicesView, "invoices", "view"),
            new PermissionSeed(PermissionCodes.InvoicesManage, "invoices", "manage"),
            new PermissionSeed(PermissionCodes.PickListsView, "pick_lists", "view"),
            new PermissionSeed(PermissionCodes.PickListsManage, "pick_lists", "manage"),
            new PermissionSeed(PermissionCodes.PickListsVerify, "pick_lists", "verify"),
            new PermissionSeed(PermissionCodes.DeliveriesView, "deliveries", "view"),
            new PermissionSeed(PermissionCodes.DeliveriesManage, "deliveries", "manage"),
            new PermissionSeed(PermissionCodes.DeliveriesDispatch, "deliveries", "dispatch"),
            new PermissionSeed(PermissionCodes.ReturnsView, "returns", "view"),
            new PermissionSeed(PermissionCodes.ReturnsManage, "returns", "manage"),
            new PermissionSeed(PermissionCodes.ReturnsResolve, "returns", "resolve"),
            new PermissionSeed(PermissionCodes.CreditNotesView, "credit_notes", "view"),
            new PermissionSeed(PermissionCodes.PaymentsView, "payments", "view"),
            new PermissionSeed(PermissionCodes.PaymentsManage, "payments", "manage"),
            new PermissionSeed(PermissionCodes.PaymentsAllocate, "payments", "allocate"),
            new PermissionSeed(PermissionCodes.PaymentsReverse, "payments", "reverse"),
            new PermissionSeed(PermissionCodes.TargetsView, "kpi_targets", "view"),
            new PermissionSeed(PermissionCodes.TargetsManage, "kpi_targets", "manage"),
            new PermissionSeed(PermissionCodes.RewardsView, "kpi_rewards", "view"),
            new PermissionSeed(PermissionCodes.RewardsManage, "kpi_rewards", "manage"),
            new PermissionSeed(PermissionCodes.RewardsApprove, "kpi_rewards", "approve"),
            new PermissionSeed(PermissionCodes.AnalyticsView, "analytics", "view"),
            new PermissionSeed(PermissionCodes.ApprovalsView, "approvals", "view"),
            new PermissionSeed(PermissionCodes.ApprovalsManage, "approvals", "manage"),
            new PermissionSeed(PermissionCodes.ApprovalsAct, "approvals", "act"),
            new PermissionSeed(PermissionCodes.NotificationsView, "notifications", "view"),
            new PermissionSeed(PermissionCodes.NotificationsManage, "notifications", "manage"),
            new PermissionSeed(PermissionCodes.AuditLogsView, "audit_logs", "view"),
            new PermissionSeed(PermissionCodes.FilesView, "files", "view"),
            new PermissionSeed(PermissionCodes.FilesManage, "files", "manage"),
            new PermissionSeed(PermissionCodes.SettingsView, "settings", "view"),
            new PermissionSeed(PermissionCodes.SettingsManage, "settings", "manage"),
            new PermissionSeed(PermissionCodes.SystemChecksRun, "system_checks", "run"),
            new PermissionSeed(PermissionCodes.LookupsView, "lookups", "view")
        };

        foreach (var seed in permissions)
        {
            var permission = await _db.Permissions
                .FirstOrDefaultAsync(
                    x => x.PermissionCode == seed.Code,
                    cancellationToken);

            if (permission is null)
            {
                permission = new Permission
                {
                    PermissionCode = seed.Code,
                    ModuleName = seed.Module,
                    ActionName = seed.Action,
                    Description = $"{seed.Module} {seed.Action} permission.",
                    CreatedAt = _clock.UtcNow
                };

                await _db.Permissions.AddAsync(
                    permission,
                    cancellationToken);
            }
            else
            {
                permission.ModuleName = seed.Module;
                permission.ActionName = seed.Action;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var superAdminRoleID = await _db.Roles
            .Where(x => x.RoleCode == RoleCodes.SuperAdmin)
            .Select(x => x.RoleID)
            .SingleAsync(cancellationToken);

        var permissionIDs = await _db.Permissions
            .Select(x => x.PermissionID)
            .ToArrayAsync(cancellationToken);

        var existingPermissionIDs = await _db.RolePermissions
            .Where(x => x.RoleID == superAdminRoleID)
            .Select(x => x.PermissionID)
            .ToArrayAsync(cancellationToken);

        var missingPermissionIDs =
            permissionIDs.Except(existingPermissionIDs);

        foreach (var permissionID in missingPermissionIDs)
        {
            await _db.RolePermissions.AddAsync(
                new RolePermission
                {
                    RoleID = superAdminRoleID,
                    PermissionID = permissionID,
                    IsAllowed = true,
                    CreatedAt = _clock.UtcNow
                },
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);

        var crmManagerCodes = new[]
        {
            PermissionCodes.ClientsView, PermissionCodes.ClientsManage, PermissionCodes.ClientCreditManage,
            PermissionCodes.LeadsView, PermissionCodes.LeadsManage, PermissionCodes.LeadScoreRulesManage,
            PermissionCodes.DuplicateReviewsManage, PermissionCodes.ActivitiesView, PermissionCodes.ActivitiesManage,
            PermissionCodes.TasksView, PermissionCodes.TasksManage, PermissionCodes.OpportunitiesView,
            PermissionCodes.OpportunitiesManage, PermissionCodes.QuotationsView, PermissionCodes.QuotationsManage,
            PermissionCodes.QuotationsApprove, PermissionCodes.ComplaintsView, PermissionCodes.ComplaintsManage,
            PermissionCodes.ReactivationView, PermissionCodes.ReactivationManage, PermissionCodes.CrmDashboardView
        };

        var salesCodes = new[]
        {
            PermissionCodes.ClientsView, PermissionCodes.LeadsView, PermissionCodes.LeadsManage,
            PermissionCodes.ActivitiesView, PermissionCodes.ActivitiesManage, PermissionCodes.TasksView,
            PermissionCodes.TasksManage, PermissionCodes.OpportunitiesView, PermissionCodes.OpportunitiesManage,
            PermissionCodes.QuotationsView, PermissionCodes.QuotationsManage, PermissionCodes.ComplaintsView,
            PermissionCodes.ComplaintsManage
        };

        var marketingManagerCodes = new[]
        {
            PermissionCodes.CampaignsView, PermissionCodes.CampaignsManage, PermissionCodes.CampaignsApprove,
            PermissionCodes.CampaignExpensesManage, PermissionCodes.CampaignAttributionManage,
            PermissionCodes.VisitsView, PermissionCodes.VisitsManage, PermissionCodes.SamplingView,
            PermissionCodes.SamplingManage, PermissionCodes.FeedbackView, PermissionCodes.FeedbackManage,
            PermissionCodes.MarketObservationsView, PermissionCodes.MarketObservationsManage,
            PermissionCodes.BpSellOutView, PermissionCodes.BpSellOutManage, PermissionCodes.BpSellOutVerify
        };

        var productPricingManagerCodes = new[]
        {
            PermissionCodes.ProductCategoriesView, PermissionCodes.ProductCategoriesManage,
            PermissionCodes.BrandsView, PermissionCodes.BrandsManage,
            PermissionCodes.ProductsView, PermissionCodes.ProductsManage,
            PermissionCodes.SKUsView, PermissionCodes.SKUsManage,
            PermissionCodes.PriceListsView, PermissionCodes.PriceListsManage,
            PermissionCodes.DiscountRulesView, PermissionCodes.DiscountRulesManage
        };

        var productPricingReadCodes = new[]
        {
            PermissionCodes.ProductCategoriesView, PermissionCodes.BrandsView,
            PermissionCodes.ProductsView, PermissionCodes.SKUsView,
            PermissionCodes.PriceListsView, PermissionCodes.DiscountRulesView
        };

        var fieldSalesCodes = new[]
        {
            PermissionCodes.CampaignsView, PermissionCodes.VisitsView, PermissionCodes.VisitsManage,
            PermissionCodes.SamplingView, PermissionCodes.SamplingManage, PermissionCodes.FeedbackView,
            PermissionCodes.FeedbackManage, PermissionCodes.BpSellOutView
        };

        var bpCodes = new[]
        {
            PermissionCodes.CampaignsView, PermissionCodes.VisitsView, PermissionCodes.VisitsManage,
            PermissionCodes.FeedbackView, PermissionCodes.FeedbackManage, PermissionCodes.BpSellOutView,
            PermissionCodes.BpSellOutManage
        };

        var merchandiserCodes = new[]
        {
            PermissionCodes.CampaignsView, PermissionCodes.VisitsView, PermissionCodes.VisitsManage,
            PermissionCodes.MarketObservationsView, PermissionCodes.MarketObservationsManage
        };


        var procurementCodes = new[]
        {
            PermissionCodes.SuppliersView, PermissionCodes.SuppliersManage,
            PermissionCodes.PurchaseRequisitionsView, PermissionCodes.PurchaseRequisitionsManage, PermissionCodes.PurchaseRequisitionsApprove,
            PermissionCodes.PurchaseOrdersView, PermissionCodes.PurchaseOrdersManage, PermissionCodes.PurchaseOrdersApprove,
            PermissionCodes.GoodsReceiptsView, PermissionCodes.GoodsReceiptsManage, PermissionCodes.GoodsReceiptsPost,
            PermissionCodes.PurchaseInvoicesView, PermissionCodes.PurchaseInvoicesManage, PermissionCodes.SupplierPaymentsManage,
            PermissionCodes.SupplierReturnsView, PermissionCodes.SupplierReturnsManage, PermissionCodes.SupplierReturnsPost,
            PermissionCodes.WarehousesView, PermissionCodes.StockView, PermissionCodes.StockMovementsView
        };

        var warehouseCodes = new[]
        {
            PermissionCodes.WarehousesView, PermissionCodes.WarehousesManage, PermissionCodes.StockView, PermissionCodes.StockMovementsView,
            PermissionCodes.GoodsReceiptsView, PermissionCodes.GoodsReceiptsManage, PermissionCodes.GoodsReceiptsPost,
            PermissionCodes.SupplierReturnsView, PermissionCodes.SupplierReturnsManage, PermissionCodes.SupplierReturnsPost,
            PermissionCodes.StockTransfersView, PermissionCodes.StockTransfersManage, PermissionCodes.StockTransfersApprove,
            PermissionCodes.StockAdjustmentsView, PermissionCodes.StockAdjustmentsManage, PermissionCodes.StockAdjustmentsApprove
        };


        var orderManagementCodes = new[]
        {
            PermissionCodes.ModernTradePurchaseOrdersView, PermissionCodes.ModernTradePurchaseOrdersManage, PermissionCodes.ModernTradePurchaseOrdersVerify,
            PermissionCodes.OrdersView, PermissionCodes.OrdersManage, PermissionCodes.OrdersSubmit, PermissionCodes.OrdersApprove,
            PermissionCodes.OrdersReserveStock, PermissionCodes.AppliedOffersManage, PermissionCodes.InvoicesView,
            PermissionCodes.InvoicesManage, PermissionCodes.PickListsView, PermissionCodes.PickListsManage,
            PermissionCodes.PickListsVerify, PermissionCodes.DeliveriesView, PermissionCodes.DeliveriesManage,
            PermissionCodes.DeliveriesDispatch, PermissionCodes.ReturnsView, PermissionCodes.ReturnsManage,
            PermissionCodes.ReturnsResolve, PermissionCodes.CreditNotesView, PermissionCodes.PaymentsView,
            PermissionCodes.PaymentsManage, PermissionCodes.PaymentsAllocate, PermissionCodes.PaymentsReverse
        };

        var fieldOrderCodes = new[]
        {
            PermissionCodes.OrdersView, PermissionCodes.OrdersManage, PermissionCodes.OrdersSubmit,
            PermissionCodes.InvoicesView, PermissionCodes.DeliveriesView, PermissionCodes.ReturnsView,
            PermissionCodes.ReturnsManage, PermissionCodes.PaymentsView, PermissionCodes.PaymentsManage
        };

        var mtOrderCodes = new[]
        {
            PermissionCodes.ModernTradePurchaseOrdersView, PermissionCodes.ModernTradePurchaseOrdersManage,
            PermissionCodes.OrdersView, PermissionCodes.OrdersManage, PermissionCodes.OrdersSubmit
        };

        var warehouseFulfilmentCodes = new[]
        {
            PermissionCodes.OrdersView, PermissionCodes.InvoicesView, PermissionCodes.PickListsView,
            PermissionCodes.PickListsManage, PermissionCodes.PickListsVerify, PermissionCodes.DeliveriesView,
            PermissionCodes.DeliveriesManage, PermissionCodes.DeliveriesDispatch, PermissionCodes.ReturnsView,
            PermissionCodes.ReturnsManage, PermissionCodes.ReturnsResolve, PermissionCodes.CreditNotesView
        };

        var deliveryCodes = new[]
        {
            PermissionCodes.DeliveriesView, PermissionCodes.DeliveriesManage, PermissionCodes.DeliveriesDispatch,
            PermissionCodes.ReturnsView, PermissionCodes.ReturnsManage
        };

        await GrantPermissionsAsync(RoleCodes.ProcurementOfficer, procurementCodes.Concat(productPricingReadCodes).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.WarehouseOfficer, warehouseCodes.Concat(warehouseFulfilmentCodes).Concat(productPricingReadCodes).Distinct().ToArray(), cancellationToken);

        await GrantPermissionsAsync(RoleCodes.CrmManager, crmManagerCodes.Concat(new[] { PermissionCodes.OrdersView, PermissionCodes.InvoicesView, PermissionCodes.ReturnsView, PermissionCodes.PaymentsView }).Concat(productPricingReadCodes).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.SalesManager, crmManagerCodes.Concat(marketingManagerCodes).Concat(productPricingManagerCodes).Concat(orderManagementCodes).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.SalesOfficer, salesCodes.Concat(fieldSalesCodes).Concat(fieldOrderCodes).Concat(productPricingReadCodes).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.MarketingManager, marketingManagerCodes.Concat(productPricingManagerCodes).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.ModernTradeExecutive, fieldSalesCodes.Concat(mtOrderCodes).Concat(productPricingReadCodes).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.BusinessPromoter, bpCodes, cancellationToken);
        await GrantPermissionsAsync(RoleCodes.Merchandiser, merchandiserCodes, cancellationToken);
        await GrantPermissionsAsync(RoleCodes.DeliveryOfficer, deliveryCodes, cancellationToken);

        var managementInfrastructureCodes = new[]
        {
            PermissionCodes.TargetsView, PermissionCodes.TargetsManage, PermissionCodes.RewardsView,
            PermissionCodes.RewardsManage, PermissionCodes.RewardsApprove, PermissionCodes.AnalyticsView,
            PermissionCodes.ApprovalsView, PermissionCodes.ApprovalsManage, PermissionCodes.ApprovalsAct,
            PermissionCodes.NotificationsView, PermissionCodes.NotificationsManage, PermissionCodes.AuditLogsView,
            PermissionCodes.FilesView, PermissionCodes.FilesManage, PermissionCodes.SettingsView,
            PermissionCodes.SettingsManage, PermissionCodes.SystemChecksRun, PermissionCodes.LookupsView
        };

        var commonInfrastructureCodes = new[]
        {
            PermissionCodes.NotificationsView, PermissionCodes.FilesView, PermissionCodes.LookupsView
        };

        await GrantPermissionsAsync(RoleCodes.GeneralManager, managementInfrastructureCodes, cancellationToken);
        await GrantPermissionsAsync(RoleCodes.Admin, managementInfrastructureCodes, cancellationToken);
        await GrantPermissionsAsync(RoleCodes.SalesManager, managementInfrastructureCodes.Concat(orderManagementCodes).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.CrmManager, managementInfrastructureCodes.Concat(crmManagerCodes).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.MarketingManager, managementInfrastructureCodes.Concat(marketingManagerCodes).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.ProcurementOfficer, commonInfrastructureCodes.Concat(new[] { PermissionCodes.ApprovalsView, PermissionCodes.ApprovalsAct }).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.WarehouseOfficer, commonInfrastructureCodes.Concat(new[] { PermissionCodes.ApprovalsView, PermissionCodes.ApprovalsAct }).Distinct().ToArray(), cancellationToken);
        await GrantPermissionsAsync(RoleCodes.DeliveryOfficer, deliveryCodes.Concat(commonInfrastructureCodes).Distinct().ToArray(), cancellationToken);

    }

    private async Task GrantPermissionsAsync(string roleCode, IReadOnlyCollection<string> permissionCodes, CancellationToken cancellationToken)
    {
        var roleID = await _db.Roles.Where(x => x.RoleCode == roleCode).Select(x => x.RoleID).SingleAsync(cancellationToken);
        var permissionIDs = await _db.Permissions.Where(x => permissionCodes.Contains(x.PermissionCode)).Select(x => x.PermissionID).ToListAsync(cancellationToken);
        var existing = await _db.RolePermissions.Where(x => x.RoleID == roleID && permissionIDs.Contains(x.PermissionID)).Select(x => x.PermissionID).ToListAsync(cancellationToken);
        foreach (var permissionID in permissionIDs.Except(existing))
        {
            await _db.RolePermissions.AddAsync(new RolePermission
            {
                RoleID = roleID,
                PermissionID = permissionID,
                IsAllowed = true,
                CreatedAt = _clock.UtcNow
            }, cancellationToken);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedSuperAdminAsync(
        CancellationToken cancellationToken)
    {
        var email = _configuration[
            "BootstrapAdmin:Email"]?
            .Trim()
            .ToLowerInvariant();

        var fullName = _configuration[
            "BootstrapAdmin:FullName"]?
            .Trim();

        var password = _configuration[
            "BootstrapAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(fullName))
        {
            throw new InvalidOperationException(
                "BootstrapAdmin:Email and BootstrapAdmin:FullName are required.");
        }

        var admin = await _db.Users.FirstOrDefaultAsync(
            x => x.Email == email,
            cancellationToken);

        if (admin is null)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "BootstrapAdmin:Password must be supplied through user secrets or an environment variable for the first seed.");
            }

            admin = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash =
                    _passwordHashService.HashPassword(password),
                Status = UserStatus.Active,
                MustChangePassword = true,
                AccountActivatedAt = _clock.UtcNow,
                CreatedAt = _clock.UtcNow
            };

            await _db.Users.AddAsync(
                admin,
                cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }
        else
        {
            admin.FullName = fullName;
            admin.Status = UserStatus.Active;

            if (!string.IsNullOrWhiteSpace(password))
            {
                admin.PasswordHash =
                    _passwordHashService.HashPassword(password);

                admin.FailedLoginCount = 0;
                admin.LockoutEndAt = null;
                admin.MustChangePassword = true;
            }
        }

        var superAdminRoleID = await _db.Roles
            .Where(x => x.RoleCode == RoleCodes.SuperAdmin)
            .Select(x => x.RoleID)
            .SingleAsync(cancellationToken);

        var hasRole = await _db.UserRoles.AnyAsync(
            x => x.UserID == admin.UserID &&
                 x.RoleID == superAdminRoleID,
            cancellationToken);

        if (!hasRole)
        {
            await _db.UserRoles.AddAsync(
                new UserRole
                {
                    UserID = admin.UserID,
                    RoleID = superAdminRoleID,
                    AssignedAt = _clock.UtcNow,
                    AssignedByUserID = admin.UserID,
                    CreatedAt = _clock.UtcNow,
                    CreatedByUserID = admin.UserID
                },
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private sealed record RoleSeed(
        string Code,
        string Name,
        int Level);

    private sealed record PermissionSeed(
        string Code,
        string Module,
        string Action);
}
