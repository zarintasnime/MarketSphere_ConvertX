using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Entities.Procurement;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Entities.KPI;
using MarketSphere.Domain.Entities.Infrastructure;

namespace MarketSphere.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    IQueryable<Company> Companies { get; }
    IQueryable<Branch> Branches { get; }
    IQueryable<User> Users { get; }
    IQueryable<UserAccountToken> UserAccountTokens { get; }
    IQueryable<Role> Roles { get; }
    IQueryable<Permission> Permissions { get; }
    IQueryable<UserRole> UserRoles { get; }
    IQueryable<RolePermission> RolePermissions { get; }
    IQueryable<UserSession> UserSessions { get; }
    IQueryable<Designation> Designations { get; }
    IQueryable<Employee> Employees { get; }
    IQueryable<Region> Regions { get; }
    IQueryable<Area> Areas { get; }
    IQueryable<Territory> Territories { get; }
    IQueryable<Route> Routes { get; }
    IQueryable<RouteOutlet> RouteOutlets { get; }
    IQueryable<EmployeeRouteAssignment> EmployeeRouteAssignments { get; }
    IQueryable<EmployeeTerritoryAssignment> EmployeeTerritoryAssignments { get; }

    IQueryable<Client> Clients { get; }
    IQueryable<ClientContact> ClientContacts { get; }
    IQueryable<ClientCreditProfile> ClientCreditProfiles { get; }
    IQueryable<Lead> Leads { get; }
    IQueryable<LeadScoreRule> LeadScoreRules { get; }
    IQueryable<DuplicateReviewCase> DuplicateReviewCases { get; }
    IQueryable<CRMActivity> CRMActivities { get; }
    IQueryable<CRMActivityParticipant> CRMActivityParticipants { get; }
    IQueryable<CRMTask> CRMTasks { get; }
    IQueryable<Opportunity> Opportunities { get; }
    IQueryable<Quotation> Quotations { get; }
    IQueryable<QuotationItem> QuotationItems { get; }
    IQueryable<ClientSegment> ClientSegments { get; }
    IQueryable<ClientSegmentAssignment> ClientSegmentAssignments { get; }
    IQueryable<Complaint> Complaints { get; }
    IQueryable<ReactivationCase> ReactivationCases { get; }


    IQueryable<Campaign> Campaigns { get; }
    IQueryable<CampaignTarget> CampaignTargets { get; }
    IQueryable<CampaignOffer> CampaignOffers { get; }
    IQueryable<CampaignExpense> CampaignExpenses { get; }
    IQueryable<CampaignAttribution> CampaignAttributions { get; }
    IQueryable<Visit> Visits { get; }
    IQueryable<SamplingLog> SamplingLogs { get; }
    IQueryable<Feedback> Feedbacks { get; }
    IQueryable<MarketObservation> MarketObservations { get; }
    IQueryable<BPSellOut> BPSellOuts { get; }
    IQueryable<BPSellOutItem> BPSellOutItems { get; }

    IQueryable<ProductCategory> ProductCategories { get; }
    IQueryable<Brand> Brands { get; }
    IQueryable<Product> Products { get; }
    IQueryable<SKU> SKUs { get; }
    IQueryable<PriceList> PriceLists { get; }
    IQueryable<PriceListItem> PriceListItems { get; }
    IQueryable<StandardDiscountRule> StandardDiscountRules { get; }

    IQueryable<Supplier> Suppliers { get; }
    IQueryable<SupplierProduct> SupplierProducts { get; }
    IQueryable<PurchaseRequisition> PurchaseRequisitions { get; }
    IQueryable<PurchaseRequisitionItem> PurchaseRequisitionItems { get; }
    IQueryable<PurchaseOrder> PurchaseOrders { get; }
    IQueryable<PurchaseOrderItem> PurchaseOrderItems { get; }
    IQueryable<GoodsReceipt> GoodsReceipts { get; }
    IQueryable<GoodsReceiptItem> GoodsReceiptItems { get; }
    IQueryable<PurchaseInvoice> PurchaseInvoices { get; }
    IQueryable<SupplierPayment> SupplierPayments { get; }
    IQueryable<SupplierReturn> SupplierReturns { get; }
    IQueryable<SupplierReturnItem> SupplierReturnItems { get; }

    IQueryable<Warehouse> Warehouses { get; }
    IQueryable<Batch> Batches { get; }
    IQueryable<StockBalance> StockBalances { get; }
    IQueryable<StockReservation> StockReservations { get; }
    IQueryable<StockMovement> StockMovements { get; }
    IQueryable<StockTransfer> StockTransfers { get; }
    IQueryable<StockTransferItem> StockTransferItems { get; }
    IQueryable<StockAdjustment> StockAdjustments { get; }
    IQueryable<StockAdjustmentItem> StockAdjustmentItems { get; }


    IQueryable<ModernTradePurchaseOrder> ModernTradePurchaseOrders { get; }
    IQueryable<ModernTradePurchaseOrderItem> ModernTradePurchaseOrderItems { get; }
    IQueryable<Order> Orders { get; }
    IQueryable<OrderItem> OrderItems { get; }
    IQueryable<AppliedOffer> AppliedOffers { get; }
    IQueryable<Invoice> Invoices { get; }
    IQueryable<InvoiceItem> InvoiceItems { get; }
    IQueryable<PickList> PickLists { get; }
    IQueryable<PickListItem> PickListItems { get; }
    IQueryable<Delivery> Deliveries { get; }
    IQueryable<DeliveryItem> DeliveryItems { get; }
    IQueryable<ReturnRequest> ReturnRequests { get; }
    IQueryable<ReturnItem> ReturnItems { get; }
    IQueryable<CreditNote> CreditNotes { get; }
    IQueryable<Payment> Payments { get; }
    IQueryable<PaymentAllocation> PaymentAllocations { get; }



    IQueryable<EmployeeTarget> EmployeeTargets { get; }
    IQueryable<RewardRule> RewardRules { get; }
    IQueryable<RewardCalculation> RewardCalculations { get; }

    IQueryable<Notification> Notifications { get; }
    IQueryable<ApprovalPolicy> ApprovalPolicies { get; }
    IQueryable<ApprovalStepDefinition> ApprovalStepDefinitions { get; }
    IQueryable<ApprovalStepAssignee> ApprovalStepAssignees { get; }
    IQueryable<ApprovalRequest> ApprovalRequests { get; }
    IQueryable<ApprovalAction> ApprovalActions { get; }
    IQueryable<StatusHistory> StatusHistories { get; }
    IQueryable<AuditLog> AuditLogs { get; }
    IQueryable<FileAttachment> FileAttachments { get; }
    IQueryable<NumberSequence> NumberSequences { get; }
    IQueryable<SystemSetting> SystemSettings { get; }
    IQueryable<IdempotencyRequest> IdempotencyRequests { get; }
    IQueryable<OfflineSyncRecord> OfflineSyncRecords { get; }

    ValueTask AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class;

    void Remove<TEntity>(TEntity entity) where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}
