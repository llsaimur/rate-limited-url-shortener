import { Component, inject, PLATFORM_ID } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms'; // ✅ Import FormsModule
import { Client } from '../../../models/client';

@Component({
  selector: 'app-admin-client',
  standalone: true,
  imports: [CommonModule, FormsModule], // ✅ Declare FormsModule
  templateUrl: './admin-client.component.html',
})
export class AdminClientComponent {
  private api = inject(ApiService);
  private platformId = inject(PLATFORM_ID);

  clients: Client[] = [];

  // ✅ Add missing fields
  newClientName: string = '';
  newClientRateLimit: number = 100;

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      this.loadClients();
    }
  }

  loadClients() {
    this.api.getClients().subscribe({
      next: (res) => (this.clients = res),
      error: (err) => console.error('❌ Failed to load clients:', err),
    });
  }

  createClient() {
    if (!this.newClientName || this.newClientRateLimit <= 0) return;

    this.api.createClient(this.newClientName, this.newClientRateLimit).subscribe({
      next: (client) => {
        this.clients.push(client);
        this.newClientName = '';
        this.newClientRateLimit = 100;
      },
      error: (err) => console.error('❌ Failed to create client:', err),
    });
  }

  deleteClient(apiKey: string) {
    if (!confirm('Are you sure you want to delete this client?')) return;

    this.api.deleteClient(apiKey).subscribe({
      next: () => {
        this.clients = this.clients.filter((c) => c.apiKey !== apiKey);
      },
      error: (err) => console.error('❌ Failed to delete client:', err),
    });
  }
}
