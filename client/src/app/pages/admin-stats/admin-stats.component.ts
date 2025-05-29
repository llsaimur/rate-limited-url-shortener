import { Component, inject, PLATFORM_ID, ViewChild } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { Stat } from '../../../models/stat';
import { NgChartsModule, BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';

@Component({
  selector: 'app-admin-stats',
  standalone: true,
  imports: [CommonModule, NgChartsModule],
  templateUrl: './admin-stats.component.html',
})
export class AdminStatsComponent {
  private api = inject(ApiService);
  private platformId = inject(PLATFORM_ID);
  isBrowser = isPlatformBrowser(this.platformId);

  stats: Stat[] = [];

  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  chartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      { data: [], label: 'Requests' },
      { data: [], label: 'Blocked' }
    ]
  };

  chartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    plugins: {
      legend: { display: true },
    },
  };

  ngOnInit() {
    if (this.isBrowser) {
      this.loadStats();
    }
  }

  loadStats() {
    this.api.getStats().subscribe({
      next: (res) => {
        this.stats = res;
        this.chartData.labels = res.map(s => s.name);
        this.chartData.datasets[0].data = res.map(s => s.requests);
        this.chartData.datasets[1].data = res.map(s => s.blocked);

        // Force chart update (fixes UI sync issues)
        setTimeout(() => this.chart?.update());
      },
      error: (err) => console.error('❌ Failed to load stats:', err),
    });
  }
}
