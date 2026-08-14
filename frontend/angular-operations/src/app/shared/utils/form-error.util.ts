import { AbstractControl, FormGroup, ValidationErrors } from '@angular/forms';

import type { ApiValidationErrors } from '../../core/models/api-response.model';
import { normalizeValidationKey } from '../../core/models/api-response.model';

const defaultMessages: Readonly<Record<string, string>> = {
  required: 'This field is required.',
  email: 'Enter a valid email address.',
  minlength: 'The value is shorter than the minimum length.',
  maxlength: 'The value exceeds the maximum length.',
  min: 'The value is below the allowed minimum.',
  max: 'The value exceeds the allowed maximum.',
  pattern: 'The value is not in the expected format.',
  server: 'The submitted value is invalid.',
};

export function shouldShowControlError(control: AbstractControl | null): boolean {
  return Boolean(control && control.invalid && (control.dirty || control.touched));
}

export function getControlErrorMessage(
  control: AbstractControl | null,
  customMessages: Readonly<Record<string, string>> = {},
): string {
  return getValidationErrorMessage(control?.errors, customMessages);
}

export function getValidationErrorMessage(
  errors: ValidationErrors | null | undefined,
  customMessages: Readonly<Record<string, string>> = {},
): string {
  if (!errors) {
    return '';
  }

  const messages = { ...defaultMessages, ...customMessages };
  const firstErrorKey = Object.keys(errors)[0];
  const errorValue = errors[firstErrorKey];

  if (firstErrorKey === 'server' && typeof errorValue === 'string') {
    return errorValue;
  }

  return messages[firstErrorKey] ?? 'The submitted value is invalid.';
}

export function applyApiValidationErrors(
  form: FormGroup,
  errors: ApiValidationErrors | null | undefined,
): string[] {
  if (!errors) {
    return [];
  }

  const unmatchedMessages: string[] = [];

  for (const [rawKey, messages] of Object.entries(errors)) {
    const key = normalizeValidationKey(rawKey);
    const control = findControlCaseInsensitive(form, key);
    const message = messages.find((value) => value.trim().length > 0);

    if (control && message) {
      control.setErrors({ ...(control.errors ?? {}), server: message });
      control.markAsTouched();
    } else {
      unmatchedMessages.push(...messages);
    }
  }

  return unmatchedMessages;
}

export function clearServerValidationError(control: AbstractControl | null): void {
  if (!control?.errors?.['server']) {
    return;
  }

  const { server: _server, ...remaining } = control.errors;
  control.setErrors(Object.keys(remaining).length > 0 ? remaining : null);
}

function findControlCaseInsensitive(form: FormGroup, path: string): AbstractControl | null {
  const direct = form.get(path);

  if (direct) {
    return direct;
  }

  const firstSegment = path.split('.')[0].toLowerCase();
  const matchingKey = Object.keys(form.controls).find((key) => key.toLowerCase() === firstSegment);

  return matchingKey ? form.get(matchingKey) : null;
}
