import {
  BrowserRouter,
  Navigate,
  Route,
  Routes,
  useLocation,
  useNavigate,
} from "react-router-dom";

import PermissionRoute from "../auth/PermissionRoute";
import ProtectedRoute from "../auth/ProtectedRoute";
import { useAuth } from "../auth/useAuth";
import ManagementLayout from "../layout/ManagementLayout";
import ApprovalQueuePage from "../pages/ApprovalQueuePage";
import CampaignRoiPage from "../pages/CampaignRoiPage";
import Client360DrilldownPage from "../pages/Client360DrilldownPage";
import DeliveryReturnPage from "../pages/DeliveryReturnPage";
import EmployeeKpiPage from "../pages/EmployeeKpiPage";
import ExecutiveDashboardPage from "../pages/ExecutiveDashboardPage";
import GtVsMtSalesPage from "../pages/GtVsMtSalesPage";
import InventoryHealthPage from "../pages/InventoryHealthPage";
import LeadToOrderFunnelPage from "../pages/LeadToOrderFunnelPage";
import LoginPage from "../pages/LoginPage";
import NotFoundPage from "../pages/NotFoundPage";
import SellInSellOutPage from "../pages/SellInSellOutPage";

function AccessDeniedPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const { logout } = useAuth();

  const attemptedPath =
    (
      location.state as {
        attemptedPath?: string;
      } | null
    )?.attemptedPath ?? "";

  async function handleSignInAgain(): Promise<void> {
    try {
      await logout();
    } finally {
      navigate("/login", {
        replace: true,
        state: null,
      });
    }
  }

  return (
    <main className="route-state-page">
      <section className="route-state-card" role="alert">
        <span className="route-state-card__code">403</span>

        <h1>Access denied</h1>

        <p>Your account does not have the permission required for this page.</p>

        {attemptedPath ? <small>Requested route: {attemptedPath}</small> : null}

        <button
          type="button"
          className="msx-button msx-button--primary"
          onClick={() => {
            void handleSignInAgain();
          }}
        >
          Sign in with another account
        </button>
      </section>
    </main>
  );
}

function ManagementHomeRedirect() {
  const { hasPermission } = useAuth();

  if (hasPermission("analytics.view")) {
    return <Navigate to="/dashboard" replace />;
  }

  if (hasPermission("infrastructure.approvals.view")) {
    return <Navigate to="/approvals" replace />;
  }

  return <Navigate to="/access-denied" replace />;
}

export default function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route element={<ProtectedRoute />}>
          <Route path="/access-denied" element={<AccessDeniedPage />} />

          <Route element={<ManagementLayout />}>
            <Route index element={<ManagementHomeRedirect />} />

            <Route
              element={
                <PermissionRoute requiredPermissions={["analytics.view"]} />
              }
            >
              <Route path="dashboard" element={<ExecutiveDashboardPage />} />

              <Route
                path="lead-to-order-funnel"
                element={<LeadToOrderFunnelPage />}
              />

              <Route path="campaign-roi" element={<CampaignRoiPage />} />

              <Route path="gt-vs-mt-sales" element={<GtVsMtSalesPage />} />

              <Route path="sell-in-sell-out" element={<SellInSellOutPage />} />

              <Route
                path="inventory-health"
                element={<InventoryHealthPage />}
              />

              <Route path="delivery-return" element={<DeliveryReturnPage />} />

              <Route path="employee-kpi" element={<EmployeeKpiPage />} />

              <Route path="client-360" element={<Client360DrilldownPage />} />
            </Route>

            <Route
              element={
                <PermissionRoute
                  requiredPermissions={["infrastructure.approvals.view"]}
                />
              }
            >
              <Route path="approvals" element={<ApprovalQueuePage />} />
            </Route>
          </Route>

          <Route path="/management" element={<Navigate to="/" replace />} />
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  );
}
