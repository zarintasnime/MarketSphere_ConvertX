import { type SyntheticEvent, useEffect, useState } from "react";
import { Navigate, useLocation, useNavigate } from "react-router-dom";

import { getApiErrorMessage } from "../api/httpClient";
import { useAuth } from "../auth/useAuth";

interface LocationState {
  from?:
    | string
    | {
        pathname?: string;
        search?: string;
      };

  passwordChanged?: boolean;
}

const validManagementRoutes = new Set([
  "/",
  "/dashboard",
  "/lead-to-order-funnel",
  "/campaign-roi",
  "/gt-vs-mt-sales",
  "/sell-in-sell-out",
  "/inventory-health",
  "/delivery-return",
  "/employee-kpi",
  "/client-360",
  "/approvals",
  "/access-denied",
]);

function resolveRequestedPath(state: LocationState | null): string {
  let requestedPath = "/dashboard";

  if (typeof state?.from === "string") {
    requestedPath = state.from;
  } else if (state?.from) {
    requestedPath = `${state.from.pathname || ""}${state.from.search || ""}`;
  }

  if (!requestedPath) {
    return "/dashboard";
  }

  const pathname = requestedPath.split("?")[0] || "/dashboard";

  if (!validManagementRoutes.has(pathname)) {
    return "/dashboard";
  }

  return pathname === "/" ? "/dashboard" : requestedPath;
}

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();

  const { currentUser, isAuthenticated, isInitializing, login } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const state = location.state as LocationState | null;

  const requestedPath = resolveRequestedPath(state);

  useEffect(() => {
    document.title = "Sign In | MarketSphere ConvertX";
  }, []);

  if (!isInitializing && isAuthenticated && currentUser?.mustChangePassword) {
    return (
      <Navigate
        to="/change-password"
        replace
        state={{
          from: requestedPath,
        }}
      />
    );
  }

  if (!isInitializing && isAuthenticated && !currentUser?.mustChangePassword) {
    return <Navigate to={requestedPath} replace />;
  }

  async function handleSubmit(
    event: SyntheticEvent<HTMLFormElement>,
  ): Promise<void> {
    event.preventDefault();
    setErrorMessage("");

    if (!email.trim() || !password) {
      setErrorMessage("Email address and password are required.");

      return;
    }

    setIsSubmitting(true);

    try {
      const session = await login({
        email: email.trim(),
        password,
      });

      if (session.user.mustChangePassword) {
        navigate("/change-password", {
          replace: true,
          state: {
            from: requestedPath,
          },
        });

        return;
      }

      navigate(requestedPath, {
        replace: true,
      });
    } catch (error: unknown) {
      setErrorMessage(
        getApiErrorMessage(
          error,
          "Login failed. Check your email and password.",
        ),
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-page__visual" aria-hidden="true">
        <div className="auth-page__visual-content">
          <span className="auth-page__brand-mark">M</span>

          <p className="auth-page__eyebrow">MarketSphere ConvertX</p>

          <h1>Management insight with controlled access.</h1>

          <p>
            Sign in to review analytics, approvals, KPI performance, and
            business drill-down reports.
          </p>
        </div>
      </div>

      <main className="auth-page__content">
        <section className="auth-card" aria-labelledby="login-title">
          <div className="auth-card__heading">
            <span className="auth-card__mobile-brand">
              MarketSphere ConvertX
            </span>

            <h2 id="login-title">Management sign in</h2>

            <p>Use your approved MarketSphere account.</p>
          </div>

          {state?.passwordChanged ? (
            <div className="auth-alert auth-alert--success" role="status">
              Password changed successfully. Sign in again using your new
              password.
            </div>
          ) : null}

          {errorMessage ? (
            <div className="auth-alert auth-alert--danger" role="alert">
              {errorMessage}
            </div>
          ) : null}

          <form className="auth-form" onSubmit={handleSubmit} noValidate>
            <label className="msx-field">
              <span className="msx-label">Email address</span>

              <input
                autoFocus
                className="msx-input"
                type="email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                autoComplete="username"
                placeholder="name@company.com"
                maxLength={256}
              />
            </label>

            <label className="msx-field">
              <span className="msx-label">Password</span>

              <span className="auth-password-field">
                <input
                  className="msx-input"
                  type={showPassword ? "text" : "password"}
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                  autoComplete="current-password"
                  placeholder="Enter your password"
                  maxLength={200}
                />

                <button
                  type="button"
                  className="auth-password-field__toggle"
                  onClick={() => setShowPassword((value) => !value)}
                  aria-label={showPassword ? "Hide password" : "Show password"}
                >
                  {showPassword ? "Hide" : "Show"}
                </button>
              </span>
            </label>

            <button
              type="submit"
              className="msx-button msx-button--primary auth-form__submit"
              disabled={isSubmitting}
            >
              {isSubmitting ? "Signing in..." : "Sign in"}
            </button>
          </form>

          <p className="auth-card__support">
            User accounts and permissions are controlled in the Angular
            Operations Portal.
          </p>
        </section>
      </main>
    </div>
  );
}
