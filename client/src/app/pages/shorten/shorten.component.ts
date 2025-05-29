import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common'; // ✅ Needed for *ngIf
import { FormsModule } from '@angular/forms';   // ✅ Needed for ngModel, ngForm
import { ApiService } from '../../services/api.service';


@Component({
  selector: 'app-shorten',
  standalone: true,
  templateUrl: './shorten.component.html',
  imports: [CommonModule, FormsModule], // ✅ Declare here
  styleUrls: ['./shorten.component.scss']
})
export class ShortenComponent {
  originalUrl: string = '';
  shortUrl: string | null = null;

  constructor(private api: ApiService) { }

  shortenUrl() {
    this.api.shortenUrl(this.originalUrl).subscribe({
      next: (res) => {
        this.shortUrl = res.shortUrl
      },
      error: (err) => {
        console.error('❌ Failed to shorten URL:', err);
        this.shortUrl = null;
      }
    });
  }
}
