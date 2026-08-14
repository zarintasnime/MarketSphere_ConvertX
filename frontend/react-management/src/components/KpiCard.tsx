import type { ReactNode } from "react";

import type { StatusTone } from "../types/common.types";
import type { TrendDirection } from "../types/dashboard.types";

export interface KpiCardProps {
  label: string;
  value: string | number;
  hint?: string;
  trendValue?: string;
  trendDirection?: TrendDirection;
  tone?: StatusTone | "brand";
  icon?: ReactNode;
}

export default function KpiCard({
  label,
  value,
  hint,
  trendValue,
  trendDirection = "neutral",
  tone = "brand",
  icon,
}: KpiCardProps) {
  const trendSymbol =
    trendDirection === "up" ? "↑" : trendDirection === "down" ? "↓" : "•";

  return (
    <article className={`kpi-card kpi-card--${tone}`}>
      <div className="kpi-card__accent" aria-hidden="true" />

      <div className="kpi-card__content">
        <div className="kpi-card__heading">
          <p className="kpi-card__label">{label}</p>
          {icon ? <span className="kpi-card__icon">{icon}</span> : null}
        </div>

        <p className="kpi-card__value">{value}</p>

        {trendValue ? (
          <p className={`kpi-card__trend kpi-card__trend--${trendDirection}`}>
            <span aria-hidden="true">{trendSymbol}</span> {trendValue}
          </p>
        ) : hint ? (
          <p className="kpi-card__hint">{hint}</p>
        ) : null}
      </div>
    </article>
  );
}
