import { useAuth } from "../auth/useAuth";

export interface ManagementTopbarProps {
  onMenuClick: () => void;
}

function initials(name: string): string {
  return name
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase())
    .join("");
}

export default function ManagementTopbar({
  onMenuClick,
}: ManagementTopbarProps) {
  const { currentUser, logout } = useAuth();

  return (
    <header className="management-topbar">
      <button
        type="button"
        className="management-topbar__menu"
        aria-label="Open navigation"
        onClick={onMenuClick}
      >
        ☰
      </button>

      <div className="management-topbar__context">
        <span>Decision support</span>
        <strong>Management Workspace</strong>
      </div>

      <div className="management-topbar__profile">
        <span className="management-topbar__avatar" aria-hidden="true">
          {initials(currentUser?.fullName ?? "User")}
        </span>
        <div>
          <strong>{currentUser?.fullName ?? "Authenticated user"}</strong>
          <span>{currentUser?.email}</span>
        </div>
        <button
          type="button"
          className="msx-button msx-button--ghost management-topbar__logout"
          onClick={logout}
        >
          Sign out
        </button>
      </div>
    </header>
  );
}
