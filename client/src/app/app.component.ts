import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterOutlet, RouterModule } from '@angular/router'; // ✅ Add RouterModule

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule, RouterOutlet, RouterModule], // ✅ Add RouterModule here
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'UrlShortener';
  apiKey: string = '';
  adminKey: string = '';
  isBrowser: boolean;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.isBrowser = isPlatformBrowser(platformId);

    if (this.isBrowser) {
      this.apiKey = localStorage.getItem('apiKey') || '';
      this.adminKey = localStorage.getItem('adminKey') || '';
    }
  }

  saveKeys() {
    if (this.isBrowser) {
      localStorage.setItem('apiKey', this.apiKey);
      localStorage.setItem('adminKey', this.adminKey);
    }
  }
}
