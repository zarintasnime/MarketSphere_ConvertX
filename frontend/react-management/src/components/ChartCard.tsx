import type { ReactNode } from "react";

export interface ChartCardProps {
  title: string;
  subtitle?: string;
  actions?: ReactNode;
  children: ReactNode;
  className?: string;
}

export default function ChartCard({
  title,
  subtitle,
  actions,
  children,
  className = "",
}: ChartCardProps) {
  return (
    <section className={`chart-card ${className}`.trim()}>
      <header className="chart-card__header">
        <div>
          <h2>{title}</h2>
          {subtitle ? <p>{subtitle}</p> : null}
        </div>
        {actions ? <div className="chart-card__actions">{actions}</div> : null}
      </header>
      <div className="chart-card__body">{children}</div>
    </section>
  );
}
