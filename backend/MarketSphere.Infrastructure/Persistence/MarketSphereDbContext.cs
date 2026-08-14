using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Entities.Procurement;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Entities.KPI;
using MarketSphere.Domain.Entities.Infrastructure;

namespace MarketSphere.Infrastructure.Persistence;

public sealed class MarketSphereDbContext : DbContext, IApplicationDbContext
{
    public MarketSphereDbContext(DbContextOptions<MarketSphereDbContext> options) : base(options) { }

    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Branch> Branches { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserAccountToken> UserAccountTokens { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<UserSession> UserSessions { get; set; } = null!;
    public DbSet<Designation> Designations { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<Area> Areas { get; set; } = null!;
    public DbSet<Territory> Territories { get; set; } = null!;
    public DbSet<Route> Routes { get; set; } = null!;
    public DbSet<RouteOutlet> RouteOutlets { get; set; } = null!;
    public DbSet<EmployeeRouteAssignment> EmployeeRouteAssignments { get; set; } = null!;
    public DbSet<EmployeeTerritoryAssignment> EmployeeTerritoryAssignments { get; set; } = null!;

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<ClientContact> ClientContacts { get; set; } = null!;
    public DbSet<ClientCreditProfile> ClientCreditProfiles { get; set; } = null!;
    public DbSet<Lead> Leads { get; set; } = null!;
    public DbSet<LeadScoreRule> LeadScoreRules { get; set; } = null!;
    public DbSet<DuplicateReviewCase> DuplicateReviewCases { get; set; } = null!;
    public DbSet<CRMActivity> CRMActivities { get; set; } = null!;
    public DbSet<CRMActivityParticipant> CRMActivityParticipants { get; set; } = null!;
    public DbSet<CRMTask> CRMTasks { get; set; } = null!;
    public DbSet<Opportunity> Opportunities { get; set; } = null!;
    public DbSet<Quotation> Quotations { get; set; } = null!;
    public DbSet<QuotationItem> QuotationItems { get; set; } = null!;
    public DbSet<ClientSegment> ClientSegments { get; set; } = null!;
    public DbSet<ClientSegmentAssignment> ClientSegmentAssignments { get; set; } = null!;
    public DbSet<Complaint> Complaints { get; set; } = null!;
    public DbSet<ReactivationCase> ReactivationCases { get; set; } = null!;


    public DbSet<Campaign> Campaigns { get; set; } = null!;
    public DbSet<CampaignTarget> CampaignTargets { get; set; } = null!;
    public DbSet<CampaignOffer> CampaignOffers { get; set; } = null!;
    public DbSet<CampaignExpense> CampaignExpenses { get; set; } = null!;
    public DbSet<CampaignAttribution> CampaignAttributions { get; set; } = null!;
    public DbSet<Visit> Visits { get; set; } = null!;
    public DbSet<SamplingLog> SamplingLogs { get; set; } = null!;
    public DbSet<Feedback> Feedbacks { get; set; } = null!;
    public DbSet<MarketObservation> MarketObservations { get; set; } = null!;
    public DbSet<BPSellOut> BPSellOuts { get; set; } = null!;
    public DbSet<BPSellOutItem> BPSellOutItems { get; set; } = null!;

    public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<SKU> SKUs { get; set; } = null!;
    public DbSet<PriceList> PriceLists { get; set; } = null!;
    public DbSet<PriceListItem> PriceListItems { get; set; } = null!;
    public DbSet<StandardDiscountRule> StandardDiscountRules { get; set; } = null!;

    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<SupplierProduct> SupplierProducts { get; set; } = null!;
    public DbSet<PurchaseRequisition> PurchaseRequisitions { get; set; } = null!;
    public DbSet<PurchaseRequisitionItem> PurchaseRequisitionItems { get; set; } = null!;
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; } = null!;
    public DbSet<GoodsReceipt> GoodsReceipts { get; set; } = null!;
    public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; } = null!;
    public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; } = null!;
    public DbSet<SupplierPayment> SupplierPayments { get; set; } = null!;
    public DbSet<SupplierReturn> SupplierReturns { get; set; } = null!;
    public DbSet<SupplierReturnItem> SupplierReturnItems { get; set; } = null!;

    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<Batch> Batches { get; set; } = null!;
    public DbSet<StockBalance> StockBalances { get; set; } = null!;
    public DbSet<StockReservation> StockReservations { get; set; } = null!;
    public DbSet<StockMovement> StockMovements { get; set; } = null!;
    public DbSet<StockTransfer> StockTransfers { get; set; } = null!;
    public DbSet<StockTransferItem> StockTransferItems { get; set; } = null!;
    public DbSet<StockAdjustment> StockAdjustments { get; set; } = null!;
    public DbSet<StockAdjustmentItem> StockAdjustmentItems { get; set; } = null!;


    public DbSet<ModernTradePurchaseOrder> ModernTradePurchaseOrders => Set<ModernTradePurchaseOrder>();
    public DbSet<ModernTradePurchaseOrderItem> ModernTradePurchaseOrderItems => Set<ModernTradePurchaseOrderItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<AppliedOffer> AppliedOffers => Set<AppliedOffer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<PickList> PickLists => Set<PickList>();
    public DbSet<PickListItem> PickListItems => Set<PickListItem>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<DeliveryItem> DeliveryItems => Set<DeliveryItem>();
    public DbSet<ReturnRequest> ReturnRequests => Set<ReturnRequest>();
    public DbSet<ReturnItem> ReturnItems => Set<ReturnItem>();
    public DbSet<CreditNote> CreditNotes => Set<CreditNote>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentAllocation> PaymentAllocations => Set<PaymentAllocation>();


    public DbSet<EmployeeTarget> EmployeeTargets => Set<EmployeeTarget>();
    public DbSet<RewardRule> RewardRules => Set<RewardRule>();
    public DbSet<RewardCalculation> RewardCalculations => Set<RewardCalculation>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ApprovalPolicy> ApprovalPolicies => Set<ApprovalPolicy>();
    public DbSet<ApprovalStepDefinition> ApprovalStepDefinitions => Set<ApprovalStepDefinition>();
    public DbSet<ApprovalStepAssignee> ApprovalStepAssignees => Set<ApprovalStepAssignee>();
    public DbSet<ApprovalRequest> ApprovalRequests => Set<ApprovalRequest>();
    public DbSet<ApprovalAction> ApprovalActions => Set<ApprovalAction>();
    public DbSet<StatusHistory> StatusHistories => Set<StatusHistory>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<FileAttachment> FileAttachments => Set<FileAttachment>();
    public DbSet<NumberSequence> NumberSequences => Set<NumberSequence>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<IdempotencyRequest> IdempotencyRequests => Set<IdempotencyRequest>();
    public DbSet<OfflineSyncRecord> OfflineSyncRecords => Set<OfflineSyncRecord>();


    IQueryable<Company> IApplicationDbContext.Companies => Companies;
    IQueryable<Branch> IApplicationDbContext.Branches => Branches;
    IQueryable<User> IApplicationDbContext.Users => Users;
    IQueryable<UserAccountToken> IApplicationDbContext.UserAccountTokens => UserAccountTokens;
    IQueryable<Role> IApplicationDbContext.Roles => Roles;
    IQueryable<Permission> IApplicationDbContext.Permissions => Permissions;
    IQueryable<UserRole> IApplicationDbContext.UserRoles => UserRoles;
    IQueryable<RolePermission> IApplicationDbContext.RolePermissions => RolePermissions;
    IQueryable<UserSession> IApplicationDbContext.UserSessions => UserSessions;
    IQueryable<Designation> IApplicationDbContext.Designations => Designations;
    IQueryable<Employee> IApplicationDbContext.Employees => Employees;
    IQueryable<Region> IApplicationDbContext.Regions => Regions;
    IQueryable<Area> IApplicationDbContext.Areas => Areas;
    IQueryable<Territory> IApplicationDbContext.Territories => Territories;
    IQueryable<Route> IApplicationDbContext.Routes => Routes;
    IQueryable<RouteOutlet> IApplicationDbContext.RouteOutlets => RouteOutlets;
    IQueryable<EmployeeRouteAssignment> IApplicationDbContext.EmployeeRouteAssignments => EmployeeRouteAssignments;
    IQueryable<EmployeeTerritoryAssignment> IApplicationDbContext.EmployeeTerritoryAssignments => EmployeeTerritoryAssignments;
    IQueryable<Client> IApplicationDbContext.Clients => Clients;
    IQueryable<ClientContact> IApplicationDbContext.ClientContacts => ClientContacts;
    IQueryable<ClientCreditProfile> IApplicationDbContext.ClientCreditProfiles => ClientCreditProfiles;
    IQueryable<Lead> IApplicationDbContext.Leads => Leads;
    IQueryable<LeadScoreRule> IApplicationDbContext.LeadScoreRules => LeadScoreRules;
    IQueryable<DuplicateReviewCase> IApplicationDbContext.DuplicateReviewCases => DuplicateReviewCases;
    IQueryable<CRMActivity> IApplicationDbContext.CRMActivities => CRMActivities;
    IQueryable<CRMActivityParticipant> IApplicationDbContext.CRMActivityParticipants => CRMActivityParticipants;
    IQueryable<CRMTask> IApplicationDbContext.CRMTasks => CRMTasks;
    IQueryable<Opportunity> IApplicationDbContext.Opportunities => Opportunities;
    IQueryable<Quotation> IApplicationDbContext.Quotations => Quotations;
    IQueryable<QuotationItem> IApplicationDbContext.QuotationItems => QuotationItems;
    IQueryable<ClientSegment> IApplicationDbContext.ClientSegments => ClientSegments;
    IQueryable<ClientSegmentAssignment> IApplicationDbContext.ClientSegmentAssignments => ClientSegmentAssignments;
    IQueryable<Complaint> IApplicationDbContext.Complaints => Complaints;
    IQueryable<ReactivationCase> IApplicationDbContext.ReactivationCases => ReactivationCases;

    IQueryable<Campaign> IApplicationDbContext.Campaigns => Campaigns;
    IQueryable<CampaignTarget> IApplicationDbContext.CampaignTargets => CampaignTargets;
    IQueryable<CampaignOffer> IApplicationDbContext.CampaignOffers => CampaignOffers;
    IQueryable<CampaignExpense> IApplicationDbContext.CampaignExpenses => CampaignExpenses;
    IQueryable<CampaignAttribution> IApplicationDbContext.CampaignAttributions => CampaignAttributions;
    IQueryable<Visit> IApplicationDbContext.Visits => Visits;
    IQueryable<SamplingLog> IApplicationDbContext.SamplingLogs => SamplingLogs;
    IQueryable<Feedback> IApplicationDbContext.Feedbacks => Feedbacks;
    IQueryable<MarketObservation> IApplicationDbContext.MarketObservations => MarketObservations;
    IQueryable<BPSellOut> IApplicationDbContext.BPSellOuts => BPSellOuts;
    IQueryable<BPSellOutItem> IApplicationDbContext.BPSellOutItems => BPSellOutItems;

    IQueryable<ProductCategory> IApplicationDbContext.ProductCategories => ProductCategories;
    IQueryable<Brand> IApplicationDbContext.Brands => Brands;
    IQueryable<Product> IApplicationDbContext.Products => Products;
    IQueryable<SKU> IApplicationDbContext.SKUs => SKUs;
    IQueryable<PriceList> IApplicationDbContext.PriceLists => PriceLists;
    IQueryable<PriceListItem> IApplicationDbContext.PriceListItems => PriceListItems;
    IQueryable<StandardDiscountRule> IApplicationDbContext.StandardDiscountRules => StandardDiscountRules;

    IQueryable<Supplier> IApplicationDbContext.Suppliers => Suppliers;
    IQueryable<SupplierProduct> IApplicationDbContext.SupplierProducts => SupplierProducts;
    IQueryable<PurchaseRequisition> IApplicationDbContext.PurchaseRequisitions => PurchaseRequisitions;
    IQueryable<PurchaseRequisitionItem> IApplicationDbContext.PurchaseRequisitionItems => PurchaseRequisitionItems;
    IQueryable<PurchaseOrder> IApplicationDbContext.PurchaseOrders => PurchaseOrders;
    IQueryable<PurchaseOrderItem> IApplicationDbContext.PurchaseOrderItems => PurchaseOrderItems;
    IQueryable<GoodsReceipt> IApplicationDbContext.GoodsReceipts => GoodsReceipts;
    IQueryable<GoodsReceiptItem> IApplicationDbContext.GoodsReceiptItems => GoodsReceiptItems;
    IQueryable<PurchaseInvoice> IApplicationDbContext.PurchaseInvoices => PurchaseInvoices;
    IQueryable<SupplierPayment> IApplicationDbContext.SupplierPayments => SupplierPayments;
    IQueryable<SupplierReturn> IApplicationDbContext.SupplierReturns => SupplierReturns;
    IQueryable<SupplierReturnItem> IApplicationDbContext.SupplierReturnItems => SupplierReturnItems;
    IQueryable<Warehouse> IApplicationDbContext.Warehouses => Warehouses;
    IQueryable<Batch> IApplicationDbContext.Batches => Batches;
    IQueryable<StockBalance> IApplicationDbContext.StockBalances => StockBalances;
    IQueryable<StockReservation> IApplicationDbContext.StockReservations => StockReservations;
    IQueryable<StockMovement> IApplicationDbContext.StockMovements => StockMovements;
    IQueryable<StockTransfer> IApplicationDbContext.StockTransfers => StockTransfers;
    IQueryable<StockTransferItem> IApplicationDbContext.StockTransferItems => StockTransferItems;
    IQueryable<StockAdjustment> IApplicationDbContext.StockAdjustments => StockAdjustments;
    IQueryable<StockAdjustmentItem> IApplicationDbContext.StockAdjustmentItems => StockAdjustmentItems;


    IQueryable<ModernTradePurchaseOrder> IApplicationDbContext.ModernTradePurchaseOrders => ModernTradePurchaseOrders;
    IQueryable<ModernTradePurchaseOrderItem> IApplicationDbContext.ModernTradePurchaseOrderItems => ModernTradePurchaseOrderItems;
    IQueryable<Order> IApplicationDbContext.Orders => Orders;
    IQueryable<OrderItem> IApplicationDbContext.OrderItems => OrderItems;
    IQueryable<AppliedOffer> IApplicationDbContext.AppliedOffers => AppliedOffers;
    IQueryable<Invoice> IApplicationDbContext.Invoices => Invoices;
    IQueryable<InvoiceItem> IApplicationDbContext.InvoiceItems => InvoiceItems;
    IQueryable<PickList> IApplicationDbContext.PickLists => PickLists;
    IQueryable<PickListItem> IApplicationDbContext.PickListItems => PickListItems;
    IQueryable<Delivery> IApplicationDbContext.Deliveries => Deliveries;
    IQueryable<DeliveryItem> IApplicationDbContext.DeliveryItems => DeliveryItems;
    IQueryable<ReturnRequest> IApplicationDbContext.ReturnRequests => ReturnRequests;
    IQueryable<ReturnItem> IApplicationDbContext.ReturnItems => ReturnItems;
    IQueryable<CreditNote> IApplicationDbContext.CreditNotes => CreditNotes;
    IQueryable<Payment> IApplicationDbContext.Payments => Payments;
    IQueryable<PaymentAllocation> IApplicationDbContext.PaymentAllocations => PaymentAllocations;


    IQueryable<EmployeeTarget> IApplicationDbContext.EmployeeTargets => EmployeeTargets;
    IQueryable<RewardRule> IApplicationDbContext.RewardRules => RewardRules;
    IQueryable<RewardCalculation> IApplicationDbContext.RewardCalculations => RewardCalculations;
    IQueryable<Notification> IApplicationDbContext.Notifications => Notifications;
    IQueryable<ApprovalPolicy> IApplicationDbContext.ApprovalPolicies => ApprovalPolicies;
    IQueryable<ApprovalStepDefinition> IApplicationDbContext.ApprovalStepDefinitions => ApprovalStepDefinitions;
    IQueryable<ApprovalStepAssignee> IApplicationDbContext.ApprovalStepAssignees => ApprovalStepAssignees;
    IQueryable<ApprovalRequest> IApplicationDbContext.ApprovalRequests => ApprovalRequests;
    IQueryable<ApprovalAction> IApplicationDbContext.ApprovalActions => ApprovalActions;
    IQueryable<StatusHistory> IApplicationDbContext.StatusHistories => StatusHistories;
    IQueryable<AuditLog> IApplicationDbContext.AuditLogs => AuditLogs;
    IQueryable<FileAttachment> IApplicationDbContext.FileAttachments => FileAttachments;
    IQueryable<NumberSequence> IApplicationDbContext.NumberSequences => NumberSequences;
    IQueryable<SystemSetting> IApplicationDbContext.SystemSettings => SystemSettings;
    IQueryable<IdempotencyRequest> IApplicationDbContext.IdempotencyRequests => IdempotencyRequests;
    IQueryable<OfflineSyncRecord> IApplicationDbContext.OfflineSyncRecords => OfflineSyncRecords;


    async ValueTask IApplicationDbContext.AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        => await Set<TEntity>().AddAsync(entity, cancellationToken);

    void IApplicationDbContext.Remove<TEntity>(TEntity entity) => Set<TEntity>().Remove(entity);

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction is not null) return await operation(cancellationToken);
        await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketSphereDbContext).Assembly);
    }
}
