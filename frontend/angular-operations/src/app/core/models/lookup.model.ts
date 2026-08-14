export interface Lookup {
  id: number;
  code: string;
  name: string;
}

export interface SelectOption {
  value: string;
  label: string;
  disabled: boolean;
}

export function lookupToSelectOption(item: Lookup): SelectOption {
  return {
    value: String(item.id),
    label: item.name,
    disabled: false,
  };
}
