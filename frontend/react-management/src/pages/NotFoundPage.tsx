import { Link } from "react-router-dom";

export default function NotFoundPage() {
  return (
    <main className="route-state-page">
      <section className="route-state-card">
        <span className="route-state-card__code">404</span>
        <h1>Page not found</h1>
        <p>The address does not match a page in the management portal.</p>
        <Link className="msx-button msx-button--primary" to="/">
          Return to portal
        </Link>
      </section>
    </main>
  );
}
