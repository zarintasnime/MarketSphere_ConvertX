import { NavLink } from "react-router-dom";

import { useAuth } from "../auth/useAuth";

export interface ManagementSidebarProps {
  open: boolean;
  onClose: () => void;
}

interface NavigationItem {
  label: string;
  route: string;
  permission: string;
  icon: string;
}

const navigation: readonly NavigationItem[] = [
  {
    label: "Executive Dashboard",
    route: "/dashboard",
    permission: "analytics.view",
    icon: "▦",
  },
  {
    label: "Approval Queue",
    route: "/approvals",
    permission: "infrastructure.approvals.view",
    icon: "✓",
  },
  {
    label: "Lead-to-Order Funnel",
    route: "/lead-to-order-funnel",
    permission: "analytics.view",
    icon: "⇢",
  },
  {
    label: "Campaign ROI",
    route: "/campaign-roi",
    permission: "analytics.view",
    icon: "%",
  },
  {
    label: "GT vs MT Sales",
    route: "/gt-vs-mt-sales",
    permission: "analytics.view",
    icon: "↔",
  },
  {
    label: "Sell-In vs Sell-Out",
    route: "/sell-in-sell-out",
    permission: "analytics.view",
    icon: "⇅",
  },
  {
    label: "Inventory Health",
    route: "/inventory-health",
    permission: "analytics.view",
    icon: "□",
  },
  {
    label: "Delivery and Return",
    route: "/delivery-return",
    permission: "analytics.view",
    icon: "↻",
  },
  {
    label: "Employee KPI",
    route: "/employee-kpi",
    permission: "analytics.view",
    icon: "★",
  },
  {
    label: "Client 360",
    route: "/client-360",
    permission: "analytics.view",
    icon: "◉",
  },
];

export default function ManagementSidebar({
  open,
  onClose,
}: ManagementSidebarProps) {
  const { hasPermission } = useAuth();
  const visibleItems = navigation.filter((item) =>
    hasPermission(item.permission),
  );

  return (
    <>
      <button
        type="button"
        className={`sidebar-scrim${open ? " sidebar-scrim--visible" : ""}`}
        aria-label="Close navigation"
        onClick={onClose}
      />
      <aside
        className={`management-sidebar${open ? " management-sidebar--open" : ""}`}
      >
        <div className="management-sidebar__brand">
          <span className="management-sidebar__logo">MS</span>
          <div>
            <strong>MarketSphere</strong>
            <span>Management Portal</span>
          </div>
        </div>

        <nav
          className="management-sidebar__nav"
          aria-label="Management navigation"
        >
          {visibleItems.map((item) => (
            <NavLink
              key={item.route}
              to={item.route}
              end={item.route === "/dashboard"}
              onClick={onClose}
              className={({ isActive }) =>
                `management-sidebar__link${isActive ? " management-sidebar__link--active" : ""}`
              }
            >
              <span aria-hidden="true">{item.icon}</span>
              <span>{item.label}</span>
            </NavLink>
          ))}
        </nav>

        <div className="management-sidebar__footer">
          <span>Localhost academic edition</span>
          <strong>F11</strong>
        </div>
      </aside>
    </>
  );
}
