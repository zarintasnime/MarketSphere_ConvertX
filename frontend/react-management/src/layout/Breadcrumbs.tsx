import { Link, useLocation } from "react-router-dom";

const labels: Readonly<Record<string, string>> = {
  dashboard: "Executive Dashboard",
  approvals: "Approval Queue",
  "lead-to-order-funnel": "Lead-to-Order Funnel",
};

export default function Breadcrumbs() {
  const location = useLocation();
  const segments = location.pathname.split("/").filter(Boolean);

  return (
    <nav className="breadcrumbs" aria-label="Breadcrumb">
      <Link to="/">Management</Link>
      {segments.map((segment, index) => {
        const route = `/${segments.slice(0, index + 1).join("/")}`;
        const label = labels[segment] ?? segment;
        const last = index === segments.length - 1;

        return (
          <span key={route}>
            <span aria-hidden="true">/</span>
            {last ? <strong>{label}</strong> : <Link to={route}>{label}</Link>}
          </span>
        );
      })}
    </nav>
  );
}
