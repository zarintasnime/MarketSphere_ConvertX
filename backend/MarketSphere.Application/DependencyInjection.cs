using Microsoft.Extensions.DependencyInjection;
using MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;
using MarketSphere.Application.Modules.OrganizationSecurity.Services;
using MarketSphere.Application.Modules.CRM.Interfaces;
using MarketSphere.Application.Modules.CRM.Services;
using MarketSphere.Application.Modules.MarketingField.Interfaces;
using MarketSphere.Application.Modules.MarketingField.Services;
using MarketSphere.Application.Modules.ProductPricing.Interfaces;
using MarketSphere.Application.Modules.ProductPricing.Services;
using MarketSphere.Application.Modules.Procurement.Interfaces;
using MarketSphere.Application.Modules.Procurement.Services;
using MarketSphere.Application.Modules.Inventory.Interfaces;
using MarketSphere.Application.Modules.Inventory.Services;
using MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
using MarketSphere.Application.Modules.OrderFulfilment.Services;
using MarketSphere.Application.Modules.KPI.Interfaces;
using MarketSphere.Application.Modules.KPI.Services;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using MarketSphere.Application.Modules.Infrastructure.Services;

namespace MarketSphere.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IGeographyService, GeographyService>();
        services.AddScoped<IRouteService, RouteService>();

        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<ILeadService, LeadService>();
        services.AddScoped<ICrmActivityService, CrmActivityService>();
        services.AddScoped<ICrmTaskService, CrmTaskService>();
        services.AddScoped<IOpportunityService, OpportunityService>();
        services.AddScoped<IQuotationService, QuotationService>();
        services.AddScoped<IComplaintService, ComplaintService>();
        services.AddScoped<IReactivationService, ReactivationService>();
        services.AddScoped<ICrmDashboardService, CrmDashboardService>();


        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<IVisitService, VisitService>();
        services.AddScoped<IFieldWorkspaceService, FieldWorkspaceService>();
        services.AddScoped<ISamplingService, SamplingService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<IMarketObservationService, MarketObservationService>();
        services.AddScoped<IBPSellOutService, BPSellOutService>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPricingService, PricingService>();

        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IPurchaseRequisitionService, PurchaseRequisitionService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<IGoodsReceiptService, GoodsReceiptService>();
        services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();
        services.AddScoped<ISupplierReturnService, SupplierReturnService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IStockTransferService, StockTransferService>();
        services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();

        services.AddScoped<IModernTradePurchaseOrderService, ModernTradePurchaseOrderService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAppliedOfferService, AppliedOfferService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IPickListService, PickListService>();
        services.AddScoped<IDeliveryService, DeliveryService>();
        services.AddScoped<IReturnService, ReturnService>();
        services.AddScoped<IPaymentService, PaymentService>();

        services.AddScoped<ITargetService, TargetService>();
        services.AddScoped<IRewardService, RewardService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IApprovalService, ApprovalService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IFileAttachmentService, FileAttachmentService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<MarketSphere.Application.Modules.Infrastructure.Interfaces.ISystemCheckService, MarketSphere.Application.Modules.Infrastructure.Services.SystemCheckService>();
        services.AddScoped<ILookupService, LookupService>();
        return services;
    }
}
