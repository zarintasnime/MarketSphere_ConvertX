# Software Architect Review

## Executive assessment

The project has a strong enterprise domain scope and a sensible four-layer backend split (Domain, Application, Infrastructure, API). The main risk is not missing features; it is **complexity management**. With roughly 100+ domain entities, 50+ API controllers, two frontends, and many business states, the next improvements should increase consistency, test coverage, and maintainability rather than add more modules.

## What is already strong

- Domain modules are separated by business capability.
- EF Core mappings use dedicated configuration classes and migrations.
- Typed domain exceptions are mapped centrally to HTTP responses.
- JWT, roles, permissions, current-user abstraction, auditing, soft deletion, and idempotency are already represented.
- Refresh tokens are stored as hashes rather than plaintext server-side.
- Inventory/order workflows include non-trivial concepts such as reservations, batches, FEFO, status transitions, approvals, returns, and allocations.
- API responses are standardized.

## High-priority issues found

### 1. No automated test project

There is currently no dedicated unit/integration test project. For this domain, that is the biggest architectural gap because many rules are stateful and financially sensitive.

**Upgrade:** add `MarketSphere.Application.Tests` plus `MarketSphere.Api.IntegrationTests`; prioritize authorization, status transitions, stock invariants, duplicate/idempotent requests, payment allocation, and concurrency.

### 2. Async services still perform synchronous EF queries

A static scan found many synchronous LINQ materialization calls inside the Application layer even though public service methods are asynchronous. This can block request threads under load.

**Upgrade:** use `FirstOrDefaultAsync`, `SingleOrDefaultAsync`, `AnyAsync`, `CountAsync`, and `ToListAsync` consistently; then enforce this in code review/analyzers.

### 3. Time abstraction is bypassed

`IDateTimeProvider` exists, but several inventory, KPI, campaign, order-fulfilment, analytics, and system-check paths still call `DateTime.UtcNow` directly.

**Risk:** harder deterministic tests and inconsistent time behavior.

**Upgrade:** inject/use `IDateTimeProvider` everywhere business time affects logic.

### 4. Browser refresh tokens are persisted in localStorage

Both frontends store access + refresh tokens in `localStorage`. This is convenient for a portfolio app but increases impact of an XSS bug.

**Upgrade:** for production, prefer an HttpOnly + Secure + SameSite refresh cookie (or a BFF session pattern) while keeping short-lived access tokens in memory.

### 5. Refresh-token rotation needs concurrency protection

The refresh token is rotated, but the session entity does not show an optimistic concurrency token. Two near-simultaneous refresh requests can race.

**Upgrade:** add a row-version/concurrency token or atomic conditional update so a refresh token is single-use under concurrency.

### 6. Login responses can reveal account state

Unknown credentials return a generic message, while disabled/locked/invited accounts return distinct messages before/around password validation. In an internet-facing system this can help account enumeration.

**Upgrade:** return a generic authentication failure externally and log the detailed reason internally; also add rate limiting for auth endpoints.

### 7. Very large services and repeated one-line business operations

Several services are large and some inventory/order methods compress multiple validations and writes into a single line. This makes rule review harder.

**Upgrade:** one business operation per readable block; extract policy/state-transition validators and domain calculators. Use formatting/analyzers in CI.

### 8. Migration/seeding is tied to Development startup

This is fine locally but should not be the production migration strategy.

**Upgrade:** run EF migrations as a deployment/CI job or explicit admin command before app rollout.

## Logic improvements recommended

1. Introduce explicit state-machine/policy classes for Order, Delivery, Return, Purchase Order, Stock Transfer, Reward, and Approval transitions.
2. Add optimistic concurrency (`rowversion`) to stock balances, sessions, orders, payments, and other mutable financial/inventory aggregates.
3. Put transaction boundaries around multi-aggregate financial/inventory operations and verify all related services use the existing transaction abstraction consistently.
4. Standardize money rounding and currency handling in one domain utility/value object.
5. Add a consistent query specification/filter approach to reduce repeated paging/search/sort code.
6. Add structured domain event/outbox support if notifications/integrations become asynchronous.
7. Add API versioning before public/external consumers depend on the current routes.
8. Add health checks for SQL Server and storage, not only a static API health response.
9. Add rate limiting, request-size limits, and security headers for deployed environments.
10. Add OpenTelemetry traces/metrics and correlation IDs around fulfilment/inventory/payment workflows.

## Code-style improvements recommended

- Use one formatting baseline (`dotnet format`, `.editorconfig`, Prettier).
- Avoid compressed multi-statement lines in C# service methods.
- Prefer async EF operations inside async methods.
- Replace template magic numbers with named TypeScript enum members.
- Split very large frontend model/API endpoint files by feature.
- Consolidate repeated SCSS into shared tokens/mixins/components.
- Add analyzers and fail CI on warnings that represent correctness/style risks.
