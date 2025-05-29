import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class ApiKeyService {
  constructor(@Inject(PLATFORM_ID) private platformId: Object) { }

  getApiKey(): string {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('apiKey') || '';
    }
    return '';
  }

  getAdminKey(): string {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('adminKey') || '';
    }
    return '';
  }

  setApiKey(value: string) {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('apiKey', value);
    }
  }

  setAdminKey(value: string) {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('adminKey', value);
    }
  }
}
