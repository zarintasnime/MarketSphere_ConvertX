using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.KPI;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Infrastructure.Persistence.Seeders;

public sealed class DemoDataSeeder
{
    private const string DemoClientCode = "DEMO-CLIENT-001";
    private const string DemoCampaignCode = "DEMO-CAMPAIGN-001";
    private const string DemoLeadCode = "DEMO-LEAD-001";
    private const string DemoOpportunityCode = "DEMO-OPP-001";
    private const string DemoQuotationNo = "DEMO-QUO-001";
    private const string DemoBatchNo = "DEMO-BATCH-001";
    private const string DemoOrderNo = "DEMO-ORDER-001";
    private const string DemoInvoiceNo = "DEMO-INVOICE-001";

    private readonly MarketSphereDbContext _db;
    private readonly IDateTimeProvider _clock;

    public DemoDataSeeder(
        MarketSphereDbContext db,
        IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var userID = await _db.Users
            .OrderBy(x => x.UserID)
            .Select(x => x.UserID)
            .FirstAsync(cancellationToken);

        var employee = await _db.Employees
            .OrderBy(x => x.EmployeeID)
            .FirstAsync(cancellationToken);

        var region = await _db.Regions
            .OrderBy(x => x.RegionID)
            .FirstAsync(cancellationToken);

        var area = await _db.Areas
            .Where(x => x.RegionID == region.RegionID)
            .OrderBy(x => x.AreaID)
            .FirstAsync(cancellationToken);

        var territory = await _db.Territories
            .Where(x => x.AreaID == area.AreaID)
            .OrderBy(x => x.TerritoryID)
            .FirstAsync(cancellationToken);

        var route = await _db.Routes
            .Where(x => x.TerritoryID == territory.TerritoryID)
            .OrderBy(x => x.RouteID)
            .FirstAsync(cancellationToken);

        var warehouse = await _db.Warehouses
            .Where(x => x.WarehouseCode == "MAIN-WH")
            .SingleAsync(cancellationToken);

        var sku = await _db.SKUs
            .Where(x => x.SKUCode == "DEMO-BATCH-001")
            .SingleAsync(cancellationToken);

        var priceList = await _db.PriceLists
            .Where(x => x.PriceListCode == "DEFAULT-GT")
            .SingleAsync(cancellationToken);

        var client = await SeedClientAsync(
            userID,
            region.RegionID,
            area.AreaID,
            territory.TerritoryID,
            cancellationToken);

        await SeedClientContactAsync(
            client.ClientID,
            userID,
            cancellationToken);

        await SeedAdditionalBangladeshClientsAsync(
            userID,
            cancellationToken);

        var creditProfile = await SeedCreditProfileAsync(
            client.ClientID,
            userID,
            cancellationToken);

        await SeedRouteOutletAsync(
            route.RouteID,
            client.ClientID,
            userID,
            cancellationToken);

        var campaign = await SeedCampaignAsync(
            employee.EmployeeID,
            region.RegionID,
            sku.SKUID,
            userID,
            cancellationToken);

        var lead = await SeedLeadAsync(
            campaign.CampaignID,
            employee.EmployeeID,
            region.RegionID,
            client.ClientID,
            userID,
            cancellationToken);

        var opportunity = await SeedOpportunityAsync(
            lead.LeadID,
            client.ClientID,
            campaign.CampaignID,
            employee.EmployeeID,
            userID,
            cancellationToken);

        var quotation = await SeedQuotationAsync(
            opportunity.OpportunityID,
            client.ClientID,
            campaign.CampaignID,
            priceList.PriceListID,
            sku.SKUID,
            userID,
            cancellationToken);

        await SeedVisitAsync(
            employee.EmployeeID,
            client.ClientID,
            route.RouteID,
            campaign.CampaignID,
            userID,
            cancellationToken);

        await SeedStockAsync(
            warehouse.WarehouseID,
            sku.SKUID,
            userID,
            cancellationToken);

        var order = await SeedOrderAsync(
            client.ClientID,
            employee.EmployeeID,
            campaign.CampaignID,
            quotation.QuotationID,
            priceList.PriceListID,
            sku.SKUID,
            userID,
            cancellationToken);

        await SeedInvoiceAsync(
            order,
            client.ClientID,
            sku.SKUID,
            userID,
            cancellationToken);

        creditProfile.CurrentDue = 950m;
        creditProfile.LastReviewedAt = _clock.UtcNow;
        client.LastOrderAt = _clock.UtcNow.AddDays(-1);
        await _db.SaveChangesAsync(cancellationToken);

        await SeedEmployeeTargetAsync(
            employee.EmployeeID,
            userID,
            cancellationToken);

        await SeedNotificationAsync(
            userID,
            order.OrderID,
            cancellationToken);
    }

    private async Task<Client> SeedClientAsync(
        int userID,
        int regionID,
        int areaID,
        int territoryID,
        CancellationToken cancellationToken)
    {
        var client = await _db.Clients
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(
                x => x.ClientCode == DemoClientCode,
                cancellationToken);

        if (client is null)
        {
            client = new Client
            {
                ClientCode = DemoClientCode,
                ClientName = "Rahman General Store",
                ClientType = ClientType.Outlet,
                Channel = SalesChannel.GeneralTrade,
                Phone = "01710000001",
                Email = "demo.client@marketsphere.local",
                Address = "Mirpur, Dhaka, Bangladesh",
                GPSLat = 23.8103000m,
                GPSLng = 90.4125000m,
                RegionID = regionID,
                AreaID = areaID,
                TerritoryID = territoryID,
                LifecycleStatus = ClientLifecycleStatus.Active,
                RiskStatus = ClientRiskStatus.Normal,
                IsActive = true,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            };

            await _db.Clients.AddAsync(
                client,
                cancellationToken);
        }
        else
        {
            client.ClientName = "Rahman General Store";
            client.RegionID = regionID;
            client.AreaID = areaID;
            client.TerritoryID = territoryID;
            client.LifecycleStatus = ClientLifecycleStatus.Active;
            client.RiskStatus = ClientRiskStatus.Normal;
            client.IsActive = true;
            client.IsDeleted = false;
            client.DeletedAt = null;
            client.DeletedByUserID = null;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return client;
    }

    private async Task SeedAdditionalBangladeshClientsAsync(
        int userID,
        CancellationToken cancellationToken)
    {
        var dhaka = await _db.Regions
            .SingleAsync(x => x.RegionCode == "DHAKA", cancellationToken);
        var dhakaArea = await _db.Areas
            .SingleAsync(x => x.AreaCode == "DHAKA-METRO", cancellationToken);
        var mirpur = await _db.Territories
            .SingleAsync(x => x.TerritoryCode == "MIRPUR", cancellationToken);

        var chattogram = await _db.Regions
            .SingleAsync(x => x.RegionCode == "CHATTOGRAM", cancellationToken);
        var chattogramArea = await _db.Areas
            .SingleAsync(x => x.AreaCode == "CTG-METRO", cancellationToken);
        var agrabad = await _db.Territories
            .SingleAsync(x => x.TerritoryCode == "AGRABAD", cancellationToken);

        var seeds = new[]
        {
            new ClientSeed("BD-CLIENT-002", "Shapla General Store", "01715000002", "Mirpur-10, Dhaka", 23.8067m, 90.3687m, dhaka.RegionID, dhakaArea.AreaID, mirpur.TerritoryID, ClientType.Outlet, SalesChannel.GeneralTrade, 3),
            new ClientSeed("BD-CLIENT-003", "Pallabi Mini Mart", "01816000003", "Pallabi, Dhaka", 23.8282m, 90.3664m, dhaka.RegionID, dhakaArea.AreaID, mirpur.TerritoryID, ClientType.Outlet, SalesChannel.GeneralTrade, 8),
            new ClientSeed("BD-CLIENT-004", "Agrabad Trade Centre", "01917000004", "Agrabad C/A, Chattogram", 22.3270m, 91.8123m, chattogram.RegionID, chattogramArea.AreaID, agrabad.TerritoryID, ClientType.Distributor, SalesChannel.ModernTrade, 2),
            new ClientSeed("BD-CLIENT-005", "Halishahar Family Shop", "01618000005", "Halishahar, Chattogram", 22.3523m, 91.7867m, chattogram.RegionID, chattogramArea.AreaID, agrabad.TerritoryID, ClientType.Outlet, SalesChannel.GeneralTrade, 15),
        };

        foreach (var seed in seeds)
        {
            var entity = await _db.Clients
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.ClientCode == seed.Code, cancellationToken);

            if (entity is null)
            {
                entity = new Client
                {
                    ClientCode = seed.Code,
                    ClientName = seed.Name,
                    ClientType = seed.ClientType,
                    Channel = seed.Channel,
                    Phone = seed.Phone,
                    Address = seed.Address,
                    GPSLat = seed.Latitude,
                    GPSLng = seed.Longitude,
                    RegionID = seed.RegionID,
                    AreaID = seed.AreaID,
                    TerritoryID = seed.TerritoryID,
                    LifecycleStatus = ClientLifecycleStatus.Active,
                    RiskStatus = ClientRiskStatus.Normal,
                    LastOrderAt = _clock.UtcNow.AddDays(-seed.DaysSinceLastOrder),
                    IsActive = true,
                    CreatedAt = _clock.UtcNow,
                    CreatedByUserID = userID
                };

                await _db.Clients.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.ClientName = seed.Name;
                entity.ClientType = seed.ClientType;
                entity.Channel = seed.Channel;
                entity.Phone = seed.Phone;
                entity.Address = seed.Address;
                entity.GPSLat = seed.Latitude;
                entity.GPSLng = seed.Longitude;
                entity.RegionID = seed.RegionID;
                entity.AreaID = seed.AreaID;
                entity.TerritoryID = seed.TerritoryID;
                entity.LifecycleStatus = ClientLifecycleStatus.Active;
                entity.RiskStatus = ClientRiskStatus.Normal;
                entity.LastOrderAt = _clock.UtcNow.AddDays(-seed.DaysSinceLastOrder);
                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.DeletedAt = null;
                entity.DeletedByUserID = null;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private sealed record ClientSeed(
        string Code,
        string Name,
        string Phone,
        string Address,
        decimal Latitude,
        decimal Longitude,
        int RegionID,
        int AreaID,
        int TerritoryID,
        ClientType ClientType,
        SalesChannel Channel,
        int DaysSinceLastOrder);

    private async Task SeedClientContactAsync(
        int clientID,
        int userID,
        CancellationToken cancellationToken)
    {
        var exists = await _db.ClientContacts.AnyAsync(
            x => x.ClientID == clientID &&
                 x.IsPrimary &&
                 x.IsActive,
            cancellationToken);

        if (exists)
            return;

        await _db.ClientContacts.AddAsync(
            new ClientContact
            {
                ClientID = clientID,
                ContactName = "Md. Rahman",
                Designation = "Proprietor",
                Phone = "01710000001",
                Email = "rahman@marketsphere.local",
                IsPrimary = true,
                IsActive = true,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            },
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<ClientCreditProfile> SeedCreditProfileAsync(
        int clientID,
        int userID,
        CancellationToken cancellationToken)
    {
        var profile = await _db.ClientCreditProfiles
            .SingleOrDefaultAsync(
                x => x.ClientID == clientID,
                cancellationToken);

        if (profile is null)
        {
            profile = new ClientCreditProfile
            {
                ClientID = clientID,
                CreditLimit = 100000m,
                CreditDays = 30,
                CurrentDue = 0m,
                IsBlocked = false,
                LastReviewedAt = _clock.UtcNow,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            };

            await _db.ClientCreditProfiles.AddAsync(
                profile,
                cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        return profile;
    }

    private async Task SeedRouteOutletAsync(
        int routeID,
        int clientID,
        int userID,
        CancellationToken cancellationToken)
    {
        var exists = await _db.RouteOutlets.AnyAsync(
            x => x.RouteID == routeID &&
                 x.ClientID == clientID &&
                 x.EffectiveTo == null,
            cancellationToken);

        if (exists)
            return;

        var lastSequence = await _db.RouteOutlets
            .Where(x => x.RouteID == routeID)
            .OrderByDescending(x => x.SequenceNo)
            .Select(x => x.SequenceNo)
            .FirstOrDefaultAsync(cancellationToken);

        await _db.RouteOutlets.AddAsync(
            new RouteOutlet
            {
                RouteID = routeID,
                ClientID = clientID,
                SequenceNo = lastSequence + 1,
                VisitFrequency = VisitFrequency.Weekly,
                EffectiveFrom = _clock.UtcToday,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            },
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Campaign> SeedCampaignAsync(
        int employeeID,
        int regionID,
        int skuID,
        int userID,
        CancellationToken cancellationToken)
    {
        var campaign = await _db.Campaigns.SingleOrDefaultAsync(
            x => x.CampaignCode == DemoCampaignCode,
            cancellationToken);

        if (campaign is null)
        {
            campaign = new Campaign
            {
                CampaignCode = DemoCampaignCode,
                CampaignTitle = "Summer Retail Growth Campaign",
                Objective = "Increase General Trade sales and client visits.",
                Budget = 100000m,
                StartDate = _clock.UtcToday.AddDays(-7),
                EndDate = _clock.UtcToday.AddDays(30),
                Channel = SalesChannel.GeneralTrade,
                Status = CampaignStatus.Active,
                CreatedByEmployeeID = employeeID,
                ActualExpense = 5000m,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            };

            await _db.Campaigns.AddAsync(
                campaign,
                cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        if (!await _db.CampaignTargets.AnyAsync(
                x => x.CampaignID == campaign.CampaignID &&
                     x.TargetType == CampaignTargetType.Region &&
                     x.RegionID == regionID,
                cancellationToken))
        {
            await _db.CampaignTargets.AddAsync(
                new CampaignTarget
                {
                    CampaignID = campaign.CampaignID,
                    TargetType = CampaignTargetType.Region,
                    RegionID = regionID,
                    TargetValue = 500000m,
                    CreatedAt = _clock.UtcNow,
                    CreatedByUserID = userID
                },
                cancellationToken);
        }

        if (!await _db.CampaignOffers.AnyAsync(
                x => x.CampaignID == campaign.CampaignID &&
                     x.OfferCode == "DEMO-5PCT",
                cancellationToken))
        {
            await _db.CampaignOffers.AddAsync(
                new CampaignOffer
                {
                    CampaignID = campaign.CampaignID,
                    OfferCode = "DEMO-5PCT",
                    OfferType = CampaignOfferType.PercentageDiscount,
                    RuleJson = "{\"minimumQuantity\":1,\"channel\":\"GeneralTrade\"}",
                    DiscountValue = 5m,
                    Priority = 10,
                    UsageLimit = 1000,
                    PerClientLimit = 10,
                    IsStackable = false,
                    IsActive = true,
                    CreatedAt = _clock.UtcNow,
                    CreatedByUserID = userID
                },
                cancellationToken);
        }

        if (!await _db.CampaignExpenses.AnyAsync(
                x => x.CampaignID == campaign.CampaignID &&
                     x.ExpenseCategory == "Digital Promotion" &&
                     x.VendorName == "Demo Media Vendor",
                cancellationToken))
        {
            await _db.CampaignExpenses.AddAsync(
                new CampaignExpense
                {
                    CampaignID = campaign.CampaignID,
                    ExpenseDate = _clock.UtcToday,
                    ExpenseCategory = "Digital Promotion",
                    Amount = 5000m,
                    VendorName = "Demo Media Vendor",
                    Description = "Seeded campaign promotion expense.",
                    Status = CampaignExpenseStatus.Posted,
                    CreatedAt = _clock.UtcNow,
                    CreatedByUserID = userID
                },
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return campaign;
    }

    private async Task<Lead> SeedLeadAsync(
        int campaignID,
        int employeeID,
        int regionID,
        int clientID,
        int userID,
        CancellationToken cancellationToken)
    {
        var lead = await _db.Leads.SingleOrDefaultAsync(
            x => x.LeadCode == DemoLeadCode,
            cancellationToken);

        if (lead is null)
        {
            lead = new Lead
            {
                LeadCode = DemoLeadCode,
                LeadName = "Rahman Retail Expansion",
                BusinessName = "Rahman General Store",
                Phone = "01710000001",
                Email = "demo.lead@marketsphere.local",
                Source = LeadSource.Campaign,
                SourceCampaignID = campaignID,
                AssignedEmployeeID = employeeID,
                RegionID = regionID,
                ProductInterest = "Demo Batch SKU",
                EstimatedValue = 50000m,
                CurrentScore = 85,
                Temperature = LeadTemperature.Hot,
                Status = LeadStatus.Converted,
                NextFollowUpAt = _clock.UtcNow.AddDays(3),
                ConvertedClientID = clientID,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            };

            await _db.Leads.AddAsync(
                lead,
                cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        return lead;
    }

    private async Task<Opportunity> SeedOpportunityAsync(
        int leadID,
        int clientID,
        int campaignID,
        int employeeID,
        int userID,
        CancellationToken cancellationToken)
    {
        var opportunity = await _db.Opportunities.SingleOrDefaultAsync(
            x => x.OpportunityCode == DemoOpportunityCode,
            cancellationToken);

        if (opportunity is null)
        {
            opportunity = new Opportunity
            {
                OpportunityCode = DemoOpportunityCode,
                LeadID = leadID,
                ClientID = clientID,
                CampaignID = campaignID,
                OwnerEmployeeID = employeeID,
                OpportunityName = "Demo Retail Order Opportunity",
                Stage = OpportunityStage.Won,
                ExpectedValue = 950m,
                ProbabilityPercent = 100,
                ExpectedCloseDate = _clock.UtcToday,
                WonAt = _clock.UtcNow,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            };

            await _db.Opportunities.AddAsync(
                opportunity,
                cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        return opportunity;
    }

    private async Task<Quotation> SeedQuotationAsync(
        int opportunityID,
        int clientID,
        int campaignID,
        int priceListID,
        int skuID,
        int userID,
        CancellationToken cancellationToken)
    {
        var quotation = await _db.Quotations.SingleOrDefaultAsync(
            x => x.QuotationNo == DemoQuotationNo &&
                 x.VersionNo == 1,
            cancellationToken);

        if (quotation is null)
        {
            quotation = new Quotation
            {
                VersionNo = 1,
                QuotationNo = DemoQuotationNo,
                OpportunityID = opportunityID,
                ClientID = clientID,
                CampaignID = campaignID,
                PriceListID = priceListID,
                ValidFrom = _clock.UtcToday.AddDays(-2),
                ValidUntil = _clock.UtcToday.AddDays(15),
                Status = QuotationStatus.Converted,
                GrossAmount = 1000m,
                DiscountAmount = 50m,
                TaxAmount = 0m,
                NetAmount = 950m,
                Terms = "30-day credit; demo quotation.",
                AcceptedAt = _clock.UtcNow.AddDays(-1),
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            };

            await _db.Quotations.AddAsync(
                quotation,
                cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        if (!await _db.QuotationItems.AnyAsync(
                x => x.QuotationID == quotation.QuotationID &&
                     x.SKUID == skuID,
                cancellationToken))
        {
            await _db.QuotationItems.AddAsync(
                new QuotationItem
                {
                    QuotationID = quotation.QuotationID,
                    SKUID = skuID,
                    Quantity = 10m,
                    UnitPrice = 100m,
                    DiscountPercent = 5m,
                    DiscountAmount = 50m,
                    TaxAmount = 0m,
                    LineTotal = 950m,
                    Note = "Seeded quotation item.",
                    CreatedAt = _clock.UtcNow,
                    CreatedByUserID = userID
                },
                cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }

        return quotation;
    }

    private async Task SeedVisitAsync(
        int employeeID,
        int clientID,
        int routeID,
        int campaignID,
        int userID,
        CancellationToken cancellationToken)
    {
        const string note = "Seeded completed field visit.";

        if (await _db.Visits.AnyAsync(
                x => x.EmployeeID == employeeID &&
                     x.ClientID == clientID &&
                     x.Note == note,
                cancellationToken))
        {
            return;
        }

        var checkIn = _clock.UtcNow.AddDays(-1).AddHours(-1);

        await _db.Visits.AddAsync(
            new Visit
            {
                EmployeeID = employeeID,
                ClientID = clientID,
                RouteID = routeID,
                CampaignID = campaignID,
                VisitType = VisitType.Sales,
                CheckInAt = checkIn,
                CheckOutAt = checkIn.AddMinutes(45),
                CheckInGPSLat = 23.8103000m,
                CheckInGPSLng = 90.4125000m,
                CheckOutGPSLat = 23.8103100m,
                CheckOutGPSLng = 90.4125100m,
                AccuracyMeters = 8m,
                IsSuspiciousLocation = false,
                Note = note,
                Status = VisitStatus.Completed,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            },
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Batch> SeedStockAsync(
        int warehouseID,
        int skuID,
        int userID,
        CancellationToken cancellationToken)
    {
        var batch = await _db.Batches.SingleOrDefaultAsync(
            x => x.SKUID == skuID &&
                 x.BatchNo == DemoBatchNo,
            cancellationToken);

        if (batch is null)
        {
            batch = new Batch
            {
                SKUID = skuID,
                BatchNo = DemoBatchNo,
                ManufacturingDate = _clock.UtcNow.AddDays(-30),
                ExpiryDate = _clock.UtcNow.AddDays(365),
                BestBeforeDate = _clock.UtcNow.AddDays(330),
                CostPrice = 80m,
                Status = BatchStatus.Available,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            };

            await _db.Batches.AddAsync(
                batch,
                cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var stockBalance = await _db.StockBalances.SingleOrDefaultAsync(
            x => x.WarehouseID == warehouseID &&
                 x.SKUID == skuID &&
                 x.BatchID == batch.BatchID,
            cancellationToken);

        if (stockBalance is null)
        {
            stockBalance = new StockBalance
            {
                WarehouseID = warehouseID,
                SKUID = skuID,
                BatchID = batch.BatchID,
                OnHandQuantity = 500m,
                ReservedQuantity = 0m,
                QuarantineQuantity = 0m,
                DamagedQuantity = 0m,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            };

            await _db.StockBalances.AddAsync(
                stockBalance,
                cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var movementExists = await _db.StockMovements.AnyAsync(
            x => x.ReferenceType == ReferenceTypeCodes.Batch &&
                 x.ReferenceID == batch.BatchID &&
                 x.MovementType == StockMovementType.AdjustmentIn,
            cancellationToken);

        if (!movementExists)
        {
            await _db.StockMovements.AddAsync(
                new StockMovement
                {
                    WarehouseID = warehouseID,
                    SKUID = skuID,
                    BatchID = batch.BatchID,
                    MovementType = StockMovementType.AdjustmentIn,
                    QuantityIn = 500m,
                    QuantityOut = 0m,
                    BalanceAfter = 500m,
                    ReferenceType = ReferenceTypeCodes.Batch,
                    ReferenceID = batch.BatchID,
                    MovementAt = _clock.UtcNow.AddDays(-5),
                    PerformedByUserID = userID,
                    Note = "Opening demo stock.",
                    CreatedAt = _clock.UtcNow,
                    CreatedByUserID = userID
                },
                cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }

        return batch;
    }

    private async Task<Order> SeedOrderAsync(
        int clientID,
        int employeeID,
        int campaignID,
        int quotationID,
        int priceListID,
        int skuID,
        int userID,
        CancellationToken cancellationToken)
    {
        var order = await _db.Orders.SingleOrDefaultAsync(
            x => x.OrderNo == DemoOrderNo,
            cancellationToken);

        if (order is null)
        {
            order = new Order
            {
                OrderNo = DemoOrderNo,
                ClientID = clientID,
                EmployeeID = employeeID,
                Channel = SalesChannel.GeneralTrade,
                OrderSource = OrderSource.Quotation,
                CampaignID = campaignID,
                QuotationID = quotationID,
                PriceListID = priceListID,
                OrderDate = _clock.UtcNow.AddDays(-1),
                RequestedDeliveryDate = _clock.UtcNow.AddDays(2),
                DeliveryAddressSnapshot = "Mirpur, Dhaka, Bangladesh",
                Status = OrderStatus.Invoiced,
                GrossAmount = 1000m,
                DiscountAmount = 50m,
                TaxAmount = 0m,
                NetAmount = 950m,
                CreditCheckStatus = CreditCheckStatus.Passed,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            };

            await _db.Orders.AddAsync(
                order,
                cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        if (!await _db.OrderItems.AnyAsync(
                x => x.OrderID == order.OrderID &&
                     x.SKUID == skuID,
                cancellationToken))
        {
            await _db.OrderItems.AddAsync(
                new OrderItem
                {
                    OrderID = order.OrderID,
                    SKUID = skuID,
                    OrderedQuantity = 10m,
                    FreeQuantity = 0m,
                    UnitPrice = 100m,
                    DiscountPercent = 5m,
                    DiscountAmount = 50m,
                    TaxAmount = 0m,
                    LineTotal = 950m,
                    ApprovedQuantity = 10m,
                    DeliveredQuantity = 0m,
                    ReturnedQuantity = 0m,
                    BackorderQuantity = 0m,
                    CreatedAt = _clock.UtcNow,
                    CreatedByUserID = userID
                },
                cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }

        return order;
    }

    private async Task SeedInvoiceAsync(
        Order order,
        int clientID,
        int skuID,
        int userID,
        CancellationToken cancellationToken)
    {
        var invoice = await _db.Invoices.SingleOrDefaultAsync(
            x => x.InvoiceNo == DemoInvoiceNo,
            cancellationToken);

        if (invoice is null)
        {
            invoice = new Invoice
            {
                InvoiceNo = DemoInvoiceNo,
                OrderID = order.OrderID,
                ClientID = clientID,
                InvoiceDate = _clock.UtcNow,
                DueDate = _clock.UtcNow.AddDays(30),
                GrossAmount = 1000m,
                DiscountAmount = 50m,
                TaxAmount = 0m,
                TotalAmount = 950m,
                PaidAmount = 0m,
                DueAmount = 950m,
                Status = InvoiceStatus.Issued,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            };

            await _db.Invoices.AddAsync(
                invoice,
                cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var orderItemID = await _db.OrderItems
            .Where(x => x.OrderID == order.OrderID &&
                        x.SKUID == skuID)
            .Select(x => x.OrderItemID)
            .SingleAsync(cancellationToken);

        if (!await _db.InvoiceItems.AnyAsync(
                x => x.InvoiceID == invoice.InvoiceID &&
                     x.OrderItemID == orderItemID,
                cancellationToken))
        {
            await _db.InvoiceItems.AddAsync(
                new InvoiceItem
                {
                    InvoiceID = invoice.InvoiceID,
                    OrderItemID = orderItemID,
                    SKUID = skuID,
                    Quantity = 10m,
                    UnitPrice = 100m,
                    DiscountAmount = 50m,
                    TaxAmount = 0m,
                    LineTotal = 950m,
                    CreatedAt = _clock.UtcNow,
                    CreatedByUserID = userID
                },
                cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task SeedEmployeeTargetAsync(
        int employeeID,
        int userID,
        CancellationToken cancellationToken)
    {
        var periodStart = new DateTime(
            _clock.UtcNow.Year,
            _clock.UtcNow.Month,
            1,
            0,
            0,
            0,
            DateTimeKind.Utc);

        var periodEnd = periodStart
            .AddMonths(1)
            .AddTicks(-1);

        var exists = await _db.EmployeeTargets.AnyAsync(
            x => x.EmployeeID == employeeID &&
                 x.TargetPeriodStart == periodStart &&
                 x.TargetPeriodEnd == periodEnd &&
                 x.TargetType == TargetType.SalesAmount &&
                 x.CampaignID == null &&
                 x.SKUID == null &&
                 x.ClientID == null,
            cancellationToken);

        if (exists)
            return;

        await _db.EmployeeTargets.AddAsync(
            new EmployeeTarget
            {
                EmployeeID = employeeID,
                TargetPeriodStart = periodStart,
                TargetPeriodEnd = periodEnd,
                TargetType = TargetType.SalesAmount,
                TargetValue = 100000m,
                TargetAmount = 100000m,
                Status = EmployeeTargetStatus.Active,
                CreatedAt = _clock.UtcNow,
                CreatedByUserID = userID
            },
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedNotificationAsync(
        int userID,
        int orderID,
        CancellationToken cancellationToken)
    {
        const string title = "MarketSphere demo data is ready";

        var exists = await _db.Notifications.AnyAsync(
            x => x.UserID == userID &&
                 x.Title == title &&
                 x.ReferenceType == ReferenceTypeCodes.Order &&
                 x.ReferenceID == orderID,
            cancellationToken);

        if (exists)
            return;

        await _db.Notifications.AddAsync(
            new Notification
            {
                UserID = userID,
                NotificationType = NotificationType.Information,
                Title = title,
                Message = "Client, campaign, lead, quotation, stock, order, invoice and KPI demo records were created.",
                Priority = NotificationPriority.Normal,
                ReferenceType = ReferenceTypeCodes.Order,
                ReferenceID = orderID,
                IsRead = false,
                CreatedAt = _clock.UtcNow,
                ExpiresAt = _clock.UtcNow.AddDays(30)
            },
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
