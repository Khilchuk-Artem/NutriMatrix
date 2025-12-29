import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {FoodRecordsService} from '../services/food-records.service';
import {NUTRIENT_CATEGORIES} from '../../../Core/services/nutrient-categories';
import {AuthService} from '../../Auth/Services/auth.service';
import {MealRecordsService} from '../services/meal-records.service';
import {MeasureService} from '../services/measure.service';
import {HttpClient} from '@angular/common/http';
import {Calendar} from 'primeng/calendar';
import {KeyValuePipe, NgClass, NgForOf, NgIf} from '@angular/common';
import {UIChart} from 'primeng/chart';
import {DropdownModule} from 'primeng/dropdown';
import {NutrientInfo} from '../../../Core/services/nutrient.service';
import {filter, finalize, forkJoin, from, map, mergeMap, Observable, of} from 'rxjs';

@Component({
  selector: 'app-nutrient-consumption-statistics',
  imports: [
    ReactiveFormsModule,
    Calendar,
    FormsModule,
    NgClass,
    NgIf,
    UIChart,
    DropdownModule,
    KeyValuePipe,
    NgForOf
  ],
  templateUrl: './nutrient-consumption-statistics.component.html',
  standalone: true,
  styleUrl: './nutrient-consumption-statistics.component.css'
})
export class NutrientConsumptionStatisticsComponent implements OnInit {
  dateRangeForm: FormGroup;
  trackedNutrients: any[] = [];
  statistics: any[] = [];
  dailyConsumption: Map<string, Map<number, number>> = new Map();
  nutrientMetadata: NutrientInfo[] = [];
  categories = NUTRIENT_CATEGORIES;
  selectedNutrientId: number = 203;
  barChartData: any;
  lineChartData: any;
  totalSummary: any[] = [];
  barChartOptions = { scales: { y: { beginAtZero: true } } }; // Removed max: 150 to allow auto-scaling
  lineChartOptions = { scales: { y: { beginAtZero: true } } };
  isLoading = false;
  errorMessage: string | null = null;
  dropdownOptions: { label: string; value: number }[] = [];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private foodRecordService: FoodRecordsService,
    private mealRecordService: MealRecordsService,
    private measureService: MeasureService,
    private http: HttpClient,
    private cdr: ChangeDetectorRef
  ) {
    this.dateRangeForm = this.fb.group({
      startDate: [new Date(new Date().setDate(new Date().getDate() - 6))],
      endDate: [new Date()]
    });
  }

  ngOnInit(): void {
    this.loadNutrientMetadata();
  }

  loadNutrientMetadata(): void {
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe({
      next: (data) => {
        this.nutrientMetadata = data;
        this.loadTrackedNutrients();
      },
      error: () => this.errorMessage = 'Failed to load nutrient metadata.'
    });
  }

  loadTrackedNutrients(): void {
    const user = this.authService.getUser();
    if (user && 'nutrientsToTrack' in user) {
      this.trackedNutrients = user.nutrientsToTrack.filter(rec => rec.isActive);
      console.log('Tracked Nutrients:', this.trackedNutrients); // Check tracked nutrients
      this.fetchStatistics();
    }
  }

  fetchStatistics(): void {
    this.isLoading = true;
    this.errorMessage = null;
    const startDate = new Date(this.dateRangeForm.get('startDate')?.value);
    const endDate = new Date(this.dateRangeForm.get('endDate')?.value);
    endDate.setHours(23, 59, 59, 999);
    const days = Math.ceil((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24)) + 1;
    const userId = this.authService.getUser()?.userId || '';

    this.foodRecordService.getAllRecords(userId, true, startDate, endDate).subscribe({
      next: (foodRecords) => {
        this.mealRecordService.getAllRecords(userId, true, startDate, endDate).subscribe({
          next: (mealRecords) => {
            this.calculateStatistics(foodRecords, mealRecords, startDate, endDate, days);
            this.isLoading = false;
          },
          error: () => {
            this.errorMessage = 'Failed to load meal records.';
            this.isLoading = false;
          }
        });
      },
      error: () => {
        this.errorMessage = 'Failed to load food records.';
        this.isLoading = false;
      }
    });
  }

  private calculateStatistics(
    foodRecords: any[],
    mealRecords: any[],
    startDate: Date,
    endDate: Date,
    days: number
  ): void {
    this.isLoading = true;

    const jobs: Observable<Array<{ dateKey: string; nutrientId: number; amount: number }>>[] = [];

    // Helper to turn one record+measure into a single-array Observable
    const recordJob = (recordDate: string, recordAmount: number, measureId: number) =>
      this.measureService.getMeasureWithFood(measureId).pipe(
        map(measure => {
          const weight = measure.weightInGrams || 0;
          return measure.food.nutrients
            .filter(n => this.trackedNutrients.some(t => t.nutrientId === n.nutrientId))
            .map(nutrient => ({
              dateKey:    recordDate,
              nutrientId: nutrient.nutrientId,
              amount:     (nutrient.amount * recordAmount * weight) / 100
            }));
        })
      );

    // build jobs for all foodRecords
    foodRecords.forEach(r => {
      const dateKey = new Date(r.dateEaten).toISOString().split('T')[0];
      jobs.push(recordJob(dateKey, r.amount, r.foodMeasureId));
    });

    // build jobs for all mealRecords snapshots
    mealRecords.forEach(r => {
      const dateKey = new Date(r.dateEaten).toISOString().split('T')[0];
      r.ingredientSnapshots.forEach((snap: any) => {
        jobs.push(recordJob(dateKey, snap.amount, snap.foodMeasureId));
      });
    });

    forkJoin(jobs).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe(arraysOfContributions => {
      // flatten into one big array
      const contributions = arraysOfContributions.flat();

      // now aggregate exactly as before...
      const totalConsumption = new Map<number, number>();
      const dailyConsumption = new Map<string, Map<number, number>>();
      for (const { dateKey, nutrientId, amount } of contributions) {
        totalConsumption.set(nutrientId, (totalConsumption.get(nutrientId) || 0) + amount);
        if (!dailyConsumption.has(dateKey)) {
          dailyConsumption.set(dateKey, new Map());
        }
        const dayMap = dailyConsumption.get(dateKey)!;
        dayMap.set(nutrientId, (dayMap.get(nutrientId) || 0) + amount);
      }

      // build this.statistics, dropdownOptions, totalSummary, charts...
      this.statistics = this.trackedNutrients.map(t => {
        const total = totalConsumption.get(t.nutrientId) || 0;
        const avg   = total / days;
        const pct   = t.targetAmount > 0 ? (avg / t.targetAmount) * 100 : 0;
        return {
          nutrientId: t.nutrientId,
          total,
          average: avg,
          target: t.targetAmount,
          percentage: pct,
          label: this.getNutrientName(t.nutrientId)
        };
      });

      // dropdownOptions, totalSummary, set selectedNutrientId, dailyConsumption, updateCharts()...
      this.dropdownOptions = this.statistics.map(stat => ({
        label: stat.label,
        value: stat.nutrientId
      }));
      this.totalSummary = this.statistics.map(stat => ({
        nutrientId: stat.nutrientId,
        total: stat.total,
        totalTarget: stat.target * days,
        totalPercentage: stat.target > 0 ? (stat.total / (stat.target * days)) * 100 : 0
      }));
      this.dailyConsumption = dailyConsumption;
      if (!this.selectedNutrientId && this.dropdownOptions.length) {
        this.selectedNutrientId = this.dropdownOptions[0].value;
      }
      this.updateCharts();
    });
  }



  onApplyDateRange(): void {
    this.fetchStatistics();
  }

  getNutrientName(id: number): string {
    const match = this.nutrientMetadata.find(n => n.attr_id == id);
    return match ? match.name : 'Unknown';
  }

  getNutrientUnit(id: number): string {
    const match = this.nutrientMetadata.find(n => n.attr_id == id);
    return match ? match.unit : '';
  }

  formatNumber(value: number, decimals: number): string {
    return value.toFixed(decimals);
  }

  getStatisticsByCategory(categoryIds: number[]): any[] {
    return this.statistics.filter(stat => categoryIds.includes(stat.nutrientId));
  }

  getCategoryStyle(percentage: number): string {
    return percentage >= 90 && percentage <= 110 ? 'text-green-600' : 'text-red-600';
  }

  updateCharts(): void {
    // Log the data being passed to the bar chart
    console.log('Bar Chart Data:', {
      labels: this.statistics.map(stat => this.getNutrientName(stat.nutrientId)),
      data: this.statistics.map(stat => stat.percentage)
    });

    this.barChartData = {
      labels: this.statistics.map(stat => this.getNutrientName(stat.nutrientId) + ' (' +this.getNutrientUnit(stat.nutrientId)+')' ),
      datasets: [{
        label: 'Percentage of Target',
        data: this.statistics.map(stat => stat.percentage),
        backgroundColor: this.statistics.map((_, i) =>
          i % 2 === 0
            ? '#6A9D8C'
            : '#FFD580'
        )
      }]
    };




    this.updateLineChart();
  }

  updateLineChart(): void {
    if (!this.selectedNutrientId) return;
    const nutrientData = this.getDailyConsumptionData(this.selectedNutrientId);
    this.lineChartData = {
      labels: nutrientData.labels,
      datasets: [
        {
          label: 'Daily Consumption',
          data: nutrientData.data,
          borderColor: '#6A9D8C',
          fill: false
        },
        {
          label: 'Daily Target',
          data: Array(nutrientData.labels.length).fill(nutrientData.target),
          borderColor: '#D4A017',
          borderDash: [5, 5],
          fill: false
        }
      ]
    };
  }

  getDailyConsumptionData(nutrientId: number): { labels: string[], data: number[], target: number } {
    const startDate = new Date(this.dateRangeForm.get('startDate')?.value);
    const endDate = new Date(this.dateRangeForm.get('endDate')?.value);
    const labels: string[] = [];
    const data: number[] = [];
    const target = this.statistics.find(s => s.nutrientId === nutrientId)?.target || 0;

    for (let d = new Date(startDate); d <= endDate; d.setDate(d.getDate() + 1)) {
      const dateKey = d.toISOString().split('T')[0];
      labels.push(dateKey);
      data.push(this.dailyConsumption.get(dateKey)?.get(nutrientId) || 0);
    }
    return { labels, data, target };
  }

  onNutrientSelect(id: number): void {
    this.selectedNutrientId = id;
    this.updateLineChart();
  }

  currentDate: Date = new Date();
}
