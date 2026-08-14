# Review Summary

## What was improved in this package

### UI / UX
- Standardized Angular feature-table borders, sticky headers, zebra rows, hover states, overflow behavior, filter cards, inputs, focus states, action buttons, and mobile handling through the shared feature-page stylesheet.
- Polished React management tables for the same visual language.
- Added compatibility aliases for mismatched Angular `brand` vs `primary` theme tokens.
- Upgraded stock reservation and stock movement tables to display warehouse names, SKU codes/names, and batch numbers instead of relying on raw foreign-key IDs.
- Kept IDs as secondary metadata where they are useful for support/debugging.

### Bangladesh demo data
- Dhaka and Chattogram regions; Mirpur and Agrabad territories/routes.
- Tejgaon and Dhaka return warehouses with local addresses.
- Fictional Bangladesh-style suppliers and retail/distributor clients.
- BDT price list and beverage SKUs with local-market-style pricing.
- Seeders update known demo records when re-run instead of blindly creating duplicates.

### Architecture / code quality
- Removed committed JWT/admin secret values from runtime configuration.
- Added root repository hygiene files and removed Visual Studio user-specific project settings.
- Started enforcing `IDateTimeProvider` in inventory, analytics, KPI reward, campaign, and system-check code paths.
- Reformatted `StockService` into readable query/validation blocks rather than compressed statements.
- Extended inventory API DTOs to return display data needed by the UI instead of forcing clients to resolve IDs themselves.

### GitHub / run experience
- Kept the repository as a normal local-development project; Docker is not required.
- Added Windows/macOS/Linux local start scripts for the API and both frontends.
- Added a one-time Windows setup script for safe .NET User Secrets configuration.
- Added GitHub Actions CI for backend restore/build and both frontend build pipelines.
- Added a portfolio-focused README plus architecture, design, improvement-plan, and CV-entry documents.

## Main issues still recommended before calling it production-ready

1. Add unit/integration tests for authorization and core business invariants.
2. Convert remaining synchronous EF materialization in async services to async EF APIs.
3. Finish replacing direct business-time calls with `IDateTimeProvider`.
4. Add optimistic concurrency for refresh sessions and mutable stock/payment/order aggregates.
5. Move production refresh-token persistence away from browser `localStorage` toward HttpOnly/Secure cookies or a BFF/session design.
6. Replace Angular numeric status magic values with named enum constants.
7. Refactor large one-line service operations (especially stock transfer/order fulfilment) into readable policies/handlers.
8. Replace remaining relational numeric-ID inputs with searchable selectors/autocomplete.
9. Add deployment-time migrations, rate limiting, security headers, real health checks, and observability.
10. Add screenshots, architecture diagram, demo GIF/video, and test badges to the GitHub README.

## Repository scale observed during review

- 51 API controller files
- 105 domain entity source files
- Dual Angular + React frontends
- No dedicated automated test project found

This size is strong for a portfolio project, but it makes consistency and automated verification more valuable than adding additional modules.
