import { HttpParams } from '@angular/common/http';

export type QueryParamPrimitive = string | number | boolean | Date;
export type QueryParamValue =
  QueryParamPrimitive | readonly QueryParamPrimitive[] | null | undefined;

export function buildHttpParams(values: Readonly<Record<string, QueryParamValue>>): HttpParams {
  let params = new HttpParams();

  Object.entries(values).forEach(([key, value]) => {
    if (value === null || value === undefined || value === '') {
      return;
    }

    const items = Array.isArray(value) ? value : [value];

    items.forEach((item) => {
      const normalizedValue = item instanceof Date ? item.toISOString() : String(item);
      params = params.append(key, normalizedValue);
    });
  });

  return params;
}
