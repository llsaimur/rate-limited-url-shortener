import { Routes } from '@angular/router';

export const routes: Routes = [
  // Default route
  { path: '', redirectTo: 'shorten', pathMatch: 'full' },

  // Public: URL Shortener
  {
    path: 'shorten',
    loadComponent: () =>
      import('./pages/shorten/shorten.component').then(m => m.ShortenComponent)
  },

  // Admin: Stats view
  {
    path: 'admin/stats',
    loadComponent: () =>
      import('./pages/admin-stats/admin-stats.component').then(m => m.AdminStatsComponent)
  },

  // Admin: Client management
  {
    path: 'admin/clients',
    loadComponent: () =>
      import('./pages/admin-client/admin-client.component').then(m => m.AdminClientComponent)
  },

  // Catch-all fallback
  { path: '**', redirectTo: 'shorten' }
];
