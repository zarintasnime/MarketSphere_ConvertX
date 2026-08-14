import { Navigate, Outlet, useLocation } from "react-router-dom";

import LoadingPanel from "../components/LoadingPanel";
import { useAuth } from "./useAuth";

export default function ProtectedRoute() {
  const location = useLocation();
  const { currentUser, isAuthenticated, isInitializing } = useAuth();

  if (isInitializing) {
    return (
      <main className="route-state-page">
        <LoadingPanel message="Restoring your session..." />
      </main>
    );
  }

  if (!isAuthenticated) {
    return (
      <Navigate
        to="/login"
        replace
        state={{ from: location.pathname + location.search }}
      />
    );
  }

  if (currentUser?.mustChangePassword) {
    return (
      <main className="route-state-page">
        <section className="route-state-card" role="alert">
          <span className="route-state-card__code">
            PASSWORD CHANGE REQUIRED
          </span>
          <h1>Update your password first</h1>
          <p>
            Open the Angular Operations Portal and complete the password-change
            form before entering the management portal.
          </p>
          <a
            className="msx-button msx-button--primary"
            href="http://localhost:4200/auth/change-password"
          >
            Open Angular Operations Portal
          </a>
        </section>
      </main>
    );
  }

  return <Outlet />;
}
