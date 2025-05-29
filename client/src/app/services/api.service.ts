import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ApiKeyService } from './api-key.service';
import { Client } from '../../models/client';
import { Stat } from '../../models/stat';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = 'http://localhost:5002';

  constructor(private http: HttpClient, private apiKeyService: ApiKeyService) { }

  // Headers for regular API usage
  private get userHeaders(): HttpHeaders {
    const key = this.apiKeyService.getApiKey();
    return new HttpHeaders({ 'X-Api-Key': key });
  }

  // Headers for admin access
  private get adminHeaders(): HttpHeaders {
    const key = this.apiKeyService.getAdminKey();
    return new HttpHeaders({ 'X-Admin-Key': key });
  }

  // ---------- User endpoints ----------
  shortenUrl(originalUrl: string) {
    return this.http.post<{ shortUrl: string }>(
      `${this.baseUrl}/url/shorten`,
      { originalUrl },
      { headers: this.userHeaders }
    );
  }

  // ---------- Admin endpoints ----------
  getStats() {
    return this.http.get<Stat[]>(`${this.baseUrl}/admin/stats`, {
      headers: this.adminHeaders
    });
  }

  getClients() {
    return this.http.get<Client[]>(`${this.baseUrl}/admin/clients`, {
      headers: this.adminHeaders
    });
  }

  createClient(name: string, rateLimit: number) {
    return this.http.post<Client>(
      `${this.baseUrl}/admin/clients`,
      { name, rateLimit },
      { headers: this.adminHeaders }
    );
  }


  deleteClient(apiKey: string) {
    return this.http.delete(`${this.baseUrl}/admin/clients/${apiKey}`, {
      headers: this.adminHeaders
    });
  }
}
