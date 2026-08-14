# Improvement Plan

## Phase 1 — GitHub-ready baseline (applied in this package)

- Centralize and polish table UX across Angular feature screens.
- Polish React management table behavior/styles.
- Add compatibility aliases for inconsistent Angular color tokens.
- Add Bangladesh-localized demo geography, outlets, suppliers, warehouses, and products.
- Remove committed API signing/admin secrets from `appsettings.json`.
- Add root `.gitignore` and remove Visual Studio user-specific project file.
- Add safe local setup/start scripts and document the SQL Server + .NET + Node.js development workflow.
- Add GitHub Actions build workflow.
- Add project-level README and architecture/design/CV review documents.

## Phase 2 — Correctness and maintainability

1. Add backend unit + integration tests.
2. Replace synchronous EF materialization inside async services.
3. Enforce `IDateTimeProvider` for all business time.
4. Replace frontend numeric status magic values with named enums.
5. Add optimistic concurrency to mutable aggregates.
6. Refactor repeated state-transition code into policy/state-machine classes.
7. Consolidate repeated Angular page SCSS.
8. Make relational fields searchable selects instead of numeric ID inputs.

## Phase 3 — Production hardening

1. Move refresh token handling to HttpOnly/Secure cookies or BFF.
2. Add rate limiting for login/refresh and sensitive mutations.
3. Add security headers and production CORS allowlists.
4. Add deployment-time migration strategy.
5. Add SQL/storage health checks.
6. Add OpenTelemetry + structured logging dashboards.
7. Add backups, retention, data-protection/key management, and secret-manager integration.
8. Add load/concurrency tests for stock reservation, payment allocation, and token refresh.

## Phase 4 — Portfolio polish

1. Add 4–6 real screenshots/GIFs to the README.
2. Add an architecture diagram and 60–90 second demo video.
3. Add seeded demo scenarios: low stock, near expiry, overdue task, pending approval, partial delivery, return, and unpaid invoice.
4. Add a concise `docs/API_EXAMPLES.md` with login and two end-to-end workflows.
5. Add a license after deciding how you want others to use the source.

## Suggested implementation order

**Tests → async/time consistency → concurrency → enum/style cleanup → UX relational lookups → production security → observability.**

That order gives the highest risk reduction before adding more features.
