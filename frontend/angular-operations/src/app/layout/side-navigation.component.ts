import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { filter } from 'rxjs';

import { AuthService } from '../core/auth/auth.service';

interface NavigationItem {
  label: string;
  route: string;
  permission?: string;
}

interface NavigationGroup {
  key: string;
  label: string;
  items: readonly NavigationItem[];
}

@Component({
  selector: 'app-side-navigation',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './side-navigation.component.html',
  styleUrl: './side-navigation.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SideNavigationComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly activePath = signal('');
  protected readonly openGroups = signal<ReadonlySet<string>>(new Set<string>());

  protected readonly groups: readonly NavigationGroup[] = [
    {
      key: 'administration',
      label: 'Administration',
      items: [
        {
          label: 'Users',
          route: '/administration/users',
          permission: 'users.view',
        },
        {
          label: 'Roles',
          route: '/administration/roles',
          permission: 'roles.view',
        },
        {
          label: 'Employees',
          route: '/administration/employees',
          permission: 'employees.view',
        },
        {
          label: 'Settings',
          route: '/administration/settings',
          permission: 'infrastructure.settings.view',
        },
        {
          label: 'Audit Log',
          route: '/administration/audit-log',
          permission: 'infrastructure.audit_logs.view',
        },
      ],
    },
    {
      key: 'organization',
      label: 'Organization',
      items: [
        {
          label: 'Company',
          route: '/organization/company',
          permission: 'organization.view',
        },
        {
          label: 'Branches',
          route: '/organization/branches',
          permission: 'organization.view',
        },
        {
          label: 'Geography',
          route: '/organization/geography',
          permission: 'geography.view',
        },
        {
          label: 'Routes',
          route: '/organization/routes',
          permission: 'routes.view',
        },
      ],
    },
    {
      key: 'products-pricing',
      label: 'Products & Pricing',
      items: [
        {
          label: 'Categories',
          route: '/products/categories',
          permission: 'products.categories.view',
        },
        {
          label: 'Brands',
          route: '/products/brands',
          permission: 'products.brands.view',
        },
        {
          label: 'Products',
          route: '/products/list',
          permission: 'products.products.view',
        },
        {
          label: 'SKUs',
          route: '/products/skus',
          permission: 'products.skus.view',
        },
        {
          label: 'Price Lists',
          route: '/products/price-lists',
          permission: 'pricing.price_lists.view',
        },
        {
          label: 'Discount Rules',
          route: '/products/discount-rules',
          permission: 'pricing.discount_rules.view',
        },
      ],
    },
    {
      key: 'crm',
      label: 'CRM',
      items: [
        {
          label: 'Clients',
          route: '/crm/clients',
          permission: 'crm.clients.view',
        },
        {
          label: 'Leads',
          route: '/crm/leads',
          permission: 'crm.leads.view',
        },
        {
          label: 'Tasks',
          route: '/crm/tasks',
          permission: 'crm.tasks.view',
        },
        {
          label: 'Opportunities',
          route: '/crm/opportunities',
          permission: 'crm.opportunities.view',
        },
        {
          label: 'Quotations',
          route: '/crm/quotations',
          permission: 'crm.quotations.view',
        },
        {
          label: 'Complaints',
          route: '/crm/complaints',
          permission: 'crm.complaints.view',
        },
        {
          label: 'Reactivation',
          route: '/crm/reactivation',
          permission: 'crm.reactivation.view',
        },
      ],
    },
    {
      key: 'marketing',
      label: 'Marketing',
      items: [
        {
          label: 'Campaigns',
          route: '/marketing/campaigns',
          permission: 'marketing.campaigns.view',
        },
        {
          label: 'Visits',
          route: '/marketing/visits',
          permission: 'marketing.visits.view',
        },
        {
          label: 'Sampling',
          route: '/marketing/sampling',
          permission: 'marketing.sampling.view',
        },
        {
          label: 'Feedback',
          route: '/marketing/feedback',
          permission: 'marketing.feedback.view',
        },
        {
          label: 'Observations',
          route: '/marketing/market-observations',
          permission: 'marketing.market_observations.view',
        },
        {
          label: 'BP Sell-Out',
          route: '/marketing/bp-sell-out',
          permission: 'marketing.bp_sell_out.view',
        },
      ],
    },
    {
      key: 'procurement',
      label: 'Procurement',
      items: [
        {
          label: 'Suppliers',
          route: '/procurement/suppliers',
          permission: 'procurement.suppliers.view',
        },
        {
          label: 'Purchase Requisitions',
          route: '/procurement/purchase-requisitions',
          permission: 'procurement.purchase_requisitions.view',
        },
        {
          label: 'Purchase Orders',
          route: '/procurement/purchase-orders',
          permission: 'procurement.purchase_orders.view',
        },
        {
          label: 'Goods Receipts',
          route: '/procurement/goods-receipts',
          permission: 'procurement.goods_receipts.view',
        },
        {
          label: 'Purchase Invoices',
          route: '/procurement/purchase-invoices',
          permission: 'procurement.purchase_invoices.view',
        },
        {
          label: 'Supplier Returns',
          route: '/procurement/supplier-returns',
          permission: 'procurement.supplier_returns.view',
        },
      ],
    },
    {
      key: 'inventory',
      label: 'Inventory',
      items: [
        {
          label: 'Warehouses',
          route: '/inventory/warehouses',
          permission: 'inventory.warehouses.view',
        },
        {
          label: 'Stock Balance',
          route: '/inventory/stock-balance',
          permission: 'inventory.stock.view',
        },
        {
          label: 'Stock Movements',
          route: '/inventory/stock-movements',
          permission: 'inventory.stock_movements.view',
        },
        {
          label: 'Reservations',
          route: '/inventory/stock-reservations',
          permission: 'inventory.stock.view',
        },
        {
          label: 'Stock Transfers',
          route: '/inventory/stock-transfers',
          permission: 'inventory.stock_transfers.view',
        },
        {
          label: 'Stock Adjustments',
          route: '/inventory/stock-adjustments',
          permission: 'inventory.stock_adjustments.view',
        },
        {
          label: 'Batch Expiry',
          route: '/inventory/batch-expiry',
          permission: 'inventory.stock.view',
        },
      ],
    },
    {
      key: 'orders',
      label: 'Orders',
      items: [
        {
          label: 'MT Purchase Orders',
          route: '/orders/mt-purchase-orders',
          permission: 'orders.mt_purchase_orders.view',
        },
        {
          label: 'Sales Orders',
          route: '/orders/list',
          permission: 'orders.orders.view',
        },
        {
          label: 'Approval Queue',
          route: '/orders/approval-queue',
          permission: 'infrastructure.approvals.view',
        },
      ],
    },
    {
      key: 'fulfilment',
      label: 'Fulfilment',
      items: [
        {
          label: 'Invoices',
          route: '/fulfilment/invoices',
          permission: 'fulfilment.invoices.view',
        },
        {
          label: 'Pick Lists',
          route: '/fulfilment/pick-lists',
          permission: 'fulfilment.pick_lists.view',
        },
        {
          label: 'Deliveries',
          route: '/fulfilment/deliveries',
          permission: 'fulfilment.deliveries.view',
        },
      ],
    },
    {
      key: 'returns-payments',
      label: 'Returns & Payments',
      items: [
        {
          label: 'Customer Returns',
          route: '/returns-payments/returns',
          permission: 'fulfilment.returns.view',
        },
        {
          label: 'Credit Notes',
          route: '/returns-payments/credit-notes',
          permission: 'fulfilment.credit_notes.view',
        },
        {
          label: 'Customer Payments',
          route: '/returns-payments/payments',
          permission: 'payments.payments.view',
        },
      ],
    },
    {
      key: 'system',
      label: 'System',
      items: [
        {
          label: 'Notifications',
          route: '/notifications',
          permission: 'infrastructure.notifications.view',
        },
        {
          label: 'System Checks',
          route: '/notifications/system-checks',
          permission: 'infrastructure.system_checks.run',
        },
      ],
    },
    {
      key: 'field-operations',
      label: 'Field Operations',
      items: [
        {
          label: 'Field Workspace',
          route: '/field/home',
        },
      ],
    },
  ];

  constructor() {
    this.syncNavigationState(this.router.url);

    this.router.events
      .pipe(
        filter((event): event is NavigationEnd => event instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((event) => {
        this.syncNavigationState(event.urlAfterRedirects);
      });
  }

  protected canShow(item: NavigationItem): boolean {
    if (item.route.startsWith('/field/')) {
      return this.isFieldUser();
    }

    return !item.permission || this.authService.hasPermission(item.permission);
  }

  protected canShowGroup(group: NavigationGroup): boolean {
    return group.items.some((item) => this.canShow(item));
  }

  protected isGroupOpen(groupKey: string): boolean {
    return this.openGroups().has(groupKey);
  }

  protected isGroupActive(group: NavigationGroup): boolean {
    const currentPath = this.activePath();

    return group.items.some((item) => this.matchesRoute(currentPath, item.route));
  }

  protected toggleGroup(groupKey: string): void {
    this.openGroups.update((currentGroups) => {
      if (currentGroups.has(groupKey)) {
        return new Set<string>();
      }

      return new Set<string>([groupKey]);
    });
  }

  private syncNavigationState(url: string): void {
    const currentPath = this.cleanPath(url);

    this.activePath.set(currentPath);

    const activeGroup = this.groups.find((group) =>
      group.items.some((item) => this.matchesRoute(currentPath, item.route)),
    );

    if (activeGroup) {
      this.openGroups.set(new Set<string>([activeGroup.key]));
    }
  }

  private matchesRoute(currentPath: string, itemRoute: string): boolean {
    return currentPath === itemRoute || currentPath.startsWith(`${itemRoute}/`);
  }

  private cleanPath(url: string): string {
    return url.split('?')[0].split('#')[0];
  }

  private isFieldUser(): boolean {
    const roles = this.authService.currentUser()?.roleCodes ?? [];

    return ['SALES_OFFICER', 'MT_EXECUTIVE', 'BUSINESS_PROMOTER', 'MERCHANDISER'].some((role) =>
      roles.includes(role),
    );
  }
}
