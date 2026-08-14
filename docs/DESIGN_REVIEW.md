# Creative Designer / UX Review

## Overall direction

The product already has a professional blue/slate enterprise visual language, but the Angular application contains two generations of screen styling: older shared `feature-page` patterns and newer module-specific copied SCSS. That creates small differences in tables, filters, buttons, spacing, and color tokens.

## Design mismatches found

### Undefined theme token usage

A number of screens referenced `--msx-color-primary-*` while the global theme defined `--msx-color-brand-*`. This could make links/buttons render with fallback/default colors. Compatibility aliases were added so existing screens now resolve to the same brand palette.

### Table inconsistency

Some tables used the shared data-table component, while many feature screens rendered raw `<table>` markup. Table density, sticky headers, hover states, borders, and mobile overflow therefore varied.

**Improvement applied:** the shared feature-page mixin now gives raw tables the same responsive, sticky-header, zebra/hover, numeric-alignment-friendly presentation. React management tables received matching polish.

### Repeated page SCSS

Procurement/inventory pages contain near-identical form/card/grid styles. This makes future design changes expensive and increases drift.

**Next refactor:** move repeated section-card, filter-bar, actions, metrics, and line-item styles into one shared feature stylesheet and keep page SCSS only for page-specific layout.

### Action hierarchy

Some pages use plain buttons, some `msx-button`, some `link`, and some danger buttons with different backgrounds.

**Recommendation:** define four universal action styles: Primary, Secondary, Ghost/Link, Destructive. Keep destructive styling red only when the action is genuinely destructive.

### IDs shown instead of human labels

Several operational tables show values such as `#clientID`, `#warehouseID`, or `#skuID`. This is developer-friendly but not user-friendly.

**Recommendation:** API list DTOs should include display names/codes so tables show `Rahman General Store`, `Tejgaon Central Warehouse`, and `Mango Drink 250ml` with the ID as muted secondary metadata only when useful.

### Form UX

Some workflows ask users to type relational IDs manually.

**Recommendation:** use searchable selects/autocomplete for client, supplier, warehouse, SKU, order, invoice, and attachment references. Disable invalid state-transition actions instead of making the user discover errors after clicking.

## UX upgrade checklist

- Keep page title + one primary action in a consistent top header.
- Put filters in a consistent card with Search + Clear actions.
- Make tables sticky-header, horizontally scrollable, and keyboard/focus friendly.
- Show names/codes, not raw foreign keys.
- Use BDT formatting (`৳`/`BDT`) consistently for money.
- Use `dd MMM yyyy` or another single date format consistently.
- Add skeletons for dashboard-heavy pages and retain existing empty/error states.
- Add confirmation dialogs for destructive/status-final actions.
- Add success toasts after mutations, not only page reloads.
- On mobile, collapse secondary table fields into a details drawer/card.
- Add a global command/search palette only after core workflows are consistent.

## Portfolio presentation suggestion

For GitHub/CV screenshots, show four screens: executive dashboard, client 360, inventory health/stock transfer, and order fulfilment. These communicate the breadth of the system much better than a login page or basic CRUD list.
