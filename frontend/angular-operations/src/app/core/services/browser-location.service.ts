import { Injectable } from '@angular/core';

export interface BrowserLocation {
  latitude: number;
  longitude: number;
  accuracyMeters: number | null;
  capturedAt: string;
}

export interface BrowserLocationOptions {
  enableHighAccuracy?: boolean;
  timeoutMilliseconds?: number;
  maximumAgeMilliseconds?: number;
  maximumAcceptedAccuracyMeters?: number | null;
}

export type BrowserLocationErrorCode =
  | 'not_supported'
  | 'permission_denied'
  | 'position_unavailable'
  | 'timeout'
  | 'accuracy_too_low'
  | 'unknown';

export class BrowserLocationError extends Error {
  constructor(
    message: string,
    readonly code: BrowserLocationErrorCode,
    readonly retryable: boolean,
  ) {
    super(message);
    this.name = 'BrowserLocationError';
  }
}

@Injectable({ providedIn: 'root' })
export class BrowserLocationService {
  getCurrentLocation(options: BrowserLocationOptions = {}): Promise<BrowserLocation> {
    if (!('geolocation' in navigator)) {
      return Promise.reject(
        new BrowserLocationError(
          'Geolocation is not supported by this browser.',
          'not_supported',
          false,
        ),
      );
    }

    return new Promise((resolve, reject) => {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const location: BrowserLocation = {
            latitude: position.coords.latitude,
            longitude: position.coords.longitude,
            accuracyMeters: Number.isFinite(position.coords.accuracy)
              ? position.coords.accuracy
              : null,
            capturedAt: new Date(position.timestamp).toISOString(),
          };

          const maximumAccuracy = options.maximumAcceptedAccuracyMeters ?? null;

          if (
            maximumAccuracy !== null &&
            location.accuracyMeters !== null &&
            location.accuracyMeters > maximumAccuracy
          ) {
            reject(
              new BrowserLocationError(
                `Location accuracy is ${Math.round(location.accuracyMeters)} meters. Move to an open area and try again.`,
                'accuracy_too_low',
                true,
              ),
            );
            return;
          }

          resolve(location);
        },
        (error) => reject(this.toError(error)),
        {
          enableHighAccuracy: options.enableHighAccuracy ?? true,
          timeout: options.timeoutMilliseconds ?? 15000,
          maximumAge: options.maximumAgeMilliseconds ?? 0,
        },
      );
    });
  }

  formatGps(location: BrowserLocation): string {
    return `${location.latitude.toFixed(6)},${location.longitude.toFixed(6)}`;
  }

  private toError(error: GeolocationPositionError): BrowserLocationError {
    switch (error.code) {
      case error.PERMISSION_DENIED:
        return new BrowserLocationError(
          'Location permission was denied. Enable location access and try again.',
          'permission_denied',
          false,
        );
      case error.POSITION_UNAVAILABLE:
        return new BrowserLocationError(
          'The current location is unavailable.',
          'position_unavailable',
          true,
        );
      case error.TIMEOUT:
        return new BrowserLocationError(
          'Location capture timed out. Move to an open area and try again.',
          'timeout',
          true,
        );
      default:
        return new BrowserLocationError(
          'The current location could not be captured.',
          'unknown',
          true,
        );
    }
  }
}
