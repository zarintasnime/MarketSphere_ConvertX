import { Injectable } from '@angular/core';

const DEVICE_IDENTIFIER_KEY = 'marketsphere.device.identifier';

@Injectable({ providedIn: 'root' })
export class DeviceIdentifierService {
  getIdentifier(): string {
    const existing = localStorage.getItem(DEVICE_IDENTIFIER_KEY);

    if (existing) {
      return existing;
    }

    const identifier = this.createIdentifier();
    localStorage.setItem(DEVICE_IDENTIFIER_KEY, identifier);
    return identifier;
  }

  getDeviceName(): string {
    const platform = navigator.platform || navigator.userAgent;
    return platform ? `Web - ${platform}` : 'Web Browser';
  }

  private createIdentifier(): string {
    if (typeof crypto.randomUUID === 'function') {
      return crypto.randomUUID();
    }

    return `web-${Date.now()}-${Math.random().toString(36).slice(2, 14)}`;
  }
}
