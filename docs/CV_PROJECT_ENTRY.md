# CV Project Entry

## Recommended project title

**MarketSphere ConvertX — Enterprise Sales, CRM, Inventory & Distribution Platform**

## CV-ready description

Built a full-stack enterprise sales and distribution platform using **ASP.NET Core 9, EF Core, SQL Server, Angular 21, and React 19**, covering CRM, product/pricing, procurement, inventory, order fulfilment, returns/payments, field operations, approvals, KPI analytics, and administration. Implemented JWT authentication with role/permission authorization, audit/status history, soft deletion, idempotent mutations, refresh-token rotation, EF Core migrations, and Bangladesh-localized demo data.

## Strong bullet points

- Designed a modular clean-architecture backend with **100+ domain entities** and **50+ API controllers** across CRM, procurement, inventory, fulfilment, marketing, KPI, and security domains.
- Built dual frontends: an **Angular operations portal** for transactional workflows and a **React management dashboard** for executive analytics and approval monitoring.
- Implemented enterprise security patterns including **JWT authentication, hashed refresh tokens, role/permission authorization, account lockout, audit logging, and centralized exception handling**.
- Developed non-trivial business workflows for **FEFO stock reservation, batch/expiry tracking, stock transfers, purchase/order lifecycle, delivery, returns, payment allocation, campaign attribution, and KPI rewards**.
- Added automated EF Core development migrations/seeding, local run/setup scripts, and **GitHub Actions CI** to make the repository easier to clone, review, and run.
- Improved product UX with standardized responsive tables, sticky headers, consistent status/action styling, empty/loading states, and Bangladesh-specific BDT/demo data.

## Interview talking points

1. Why you separated transactional Angular UI from management React UI.
2. How idempotency protects duplicate mutations during retries/offline sync.
3. How FEFO/batch inventory differs from normal CRUD stock tables.
4. How permissions and current-user context flow from JWT to application rules.
5. What you would improve next: automated tests, optimistic concurrency, async query consistency, and HttpOnly refresh-token storage.

## GitHub short description

`Enterprise CRM, procurement, inventory, order fulfilment and analytics platform built with ASP.NET Core, EF Core, SQL Server, Angular and React.`

## Suggested repository topics

`aspnet-core`, `ef-core`, `sql-server`, `angular`, `react`, `typescript`, `clean-architecture`, `jwt-authentication`, `inventory-management`, `crm`, `sales-management`, `github-actions`
