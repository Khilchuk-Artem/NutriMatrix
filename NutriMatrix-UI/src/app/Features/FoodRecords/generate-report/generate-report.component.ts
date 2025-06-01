import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {FoodRecordDto, FoodRecordsService, NutrientInfo} from '../services/food-records.service';
import {MealRecordDto, MealRecordsService} from '../services/meal-records.service';
import {MeasureService, MeasureWithFoodDto} from '../services/measure.service';
import {MealService} from '../../FoodCatalog/Services/meal.service';
import {AuthService} from '../../Auth/Services/auth.service';
import {forkJoin, map, mergeMap, Observable, of, switchMap} from 'rxjs';
import * as Papa from 'papaparse';
import * as XLSX from 'xlsx';
import {Calendar} from 'primeng/calendar';
import {DropdownModule} from 'primeng/dropdown';
import {NgIf} from '@angular/common';
import FileSaver, { saveAs } from 'file-saver';
import {NutrientService} from '../../../Core/services/nutrient.service';
import {HttpClient} from '@angular/common/http';


interface TrackedNutrient {
  nutrientId: number;
  targetAmount: number;
  isActive: boolean;
}
interface NutrientMetaData {
  attr_id: number;
  name: string;
  unit: string;
}
interface ReportRow {
  rowNumber: number;
  dateEaten: string;
  itemName: string;
  nutrients: Record<number, number>;
}

@Component({
  selector: 'app-generate-report',
  imports: [
    ReactiveFormsModule,
    Calendar,
    DropdownModule,
    NgIf
  ],
  templateUrl: './generate-report.component.html',
  standalone: true,
  styleUrl: './generate-report.component.css'
})
export class GenerateReportComponent implements OnInit {
  reportForm: FormGroup;
  isLoading = false;
  errorMessage: string | null = null;
  trackedNutrients: TrackedNutrient[] = [];
  formatOptions = [
    { label: 'CSV', value: 'csv' },
    { label: 'XLSX', value: 'xlsx' }
  ];
  public nutrientMetadata: NutrientMetaData[] = [];

  constructor(
    private fb: FormBuilder,
    private foodRecordService: FoodRecordsService,
    private mealRecordService: MealRecordsService,
    private measureService: MeasureService,
    private authService: AuthService,
    private mealService:MealService,
    private nutrientService:NutrientService,
    private http:HttpClient
  ) {
    this.reportForm = this.fb.group({
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      format: ['csv', Validators.required]
    });
  }

  ngOnInit(): void {
    this.http.get<NutrientMetaData[]>('assets/nutrient_attributes.json')
      .subscribe(data => {
        this.nutrientMetadata = data;
      });
    const user = this.authService.getUser();
    if (user && Array.isArray((user as any).nutrientsToTrack)) {
      this.trackedNutrients = (user as any).nutrientsToTrack
        .filter((n: any) => n.isActive)
        .map((n: any) => ({
          nutrientId: n.nutrientId,
          targetAmount: n.targetAmount,
          isActive: n.isActive
        }));
    }
  }

  private getNutrientLabelPair(nutrientId: number): [string, string] {
    const match = this.nutrientMetadata.find(n => n.attr_id === nutrientId);
    console.log(nutrientId)
    if (match) {
      return [match.name, match.unit];
    }
    return [`Nutrient_${nutrientId}`, ''];
  }

  onSubmitGenerate() {
    if (this.reportForm.invalid) {
      this.reportForm.markAllAsTouched();
      return;
    }
    this.errorMessage = null;
    this.isLoading = true;
    const { startDate, endDate, format } = this.reportForm.value as {
      startDate: Date;
      endDate: Date;
      format: 'csv' | 'xlsx';
    };
    const start = new Date(startDate);
    start.setHours(0, 0, 0, 0);
    const end = new Date(endDate);
    end.setHours(23, 59, 59, 999);
    const userId = this.authService.getUser()?.userId || '';

    const foodObs: Observable<FoodRecordDto[]> = this.foodRecordService
      .getAllRecords(userId, true, start, end)
      .pipe(map(arr => arr || []));
    const mealObs: Observable<MealRecordDto[]> = this.mealRecordService
      .getAllRecords(userId, true, start, end)
      .pipe(map(arr => arr || []));

    forkJoin({ foods: foodObs, meals: mealObs })
      .pipe(
        switchMap(({ foods, meals }) => {
          const foodRows$ = foods.map(fr =>
            this.measureService.getMeasureWithFood(fr.foodMeasureId).pipe(
              map((measure: MeasureWithFoodDto) => {
                const totalGrams = (measure.weightInGrams || 0) * fr.amount;
                const nutMap: Record<number, number> = {};
                (measure.food.nutrients || []).forEach(nutr => {
                  if (!this.trackedNutrients.find(tn => tn.nutrientId === nutr.nutrientId)) {
                    return;
                  }
                  const raw = (nutr.amount * totalGrams) / 100;
                  nutMap[nutr.nutrientId] = Math.round(raw * 100) / 100;
                });
                return <ReportRow>{
                  rowNumber: -1,
                  dateEaten: fr.dateEaten,
                  itemName: measure.food.name,
                  nutrients: nutMap
                };
              })
            )
          );
          const mealRows$ = meals.map(mr =>
            forkJoin(
              mr.ingredientSnapshots.map(snap =>
                this.measureService.getMeasureWithFood(snap.foodMeasureId).pipe(
                  map((measure: MeasureWithFoodDto) => ({ snap, measure }))
                )
              )
            ).pipe(
              map(ingredients => ({
                mealId: mr.mealId,
                dateEaten: mr.dateEaten,
                ingredients
              })),
              switchMap(({ mealId, dateEaten, ingredients }) =>
                this.mealService.getMealById(mealId!).pipe(
                  map(meal => {
                    return ingredients.map(({ snap, measure }) => {
                      const totalGrams = (measure.weightInGrams || 0) * snap.amount;
                      const nutMap: Record<number, number> = {};
                      (measure.food.nutrients || []).forEach(nutr => {
                        if (!this.trackedNutrients.find(tn => tn.nutrientId === nutr.nutrientId)) {
                          return;
                        }
                        const raw = (nutr.amount * totalGrams) / 100;
                        nutMap[nutr.nutrientId] = Math.round(raw * 100) / 100;
                      });
                      const itemName = `${meal.name} → ${measure.food.name}`;
                      return <ReportRow>{
                        rowNumber: -1,
                        dateEaten,
                        itemName,
                        nutrients: nutMap
                      };
                    });
                  })
                )
              )
            )
          );
          const allFoodRows$ = foodRows$.length ? forkJoin(foodRows$) : of([] as ReportRow[]);
          const allMealRowsNested$ = mealRows$.length ? forkJoin(mealRows$) : of([] as ReportRow[][]);
          return forkJoin({ foodRows: allFoodRows$, mealRowsNested: allMealRowsNested$ });
        }),
        map(({ foodRows, mealRowsNested }) => {
          const flattenedMealRows = ([] as ReportRow[]).concat(...mealRowsNested);
          const allRows: ReportRow[] = ([...foodRows, ...flattenedMealRows] as ReportRow[]).map((r, idx) => {
            r.rowNumber = idx + 1;
            return r;
          });
          allRows.sort((a, b) => new Date(a.dateEaten).getTime() - new Date(b.dateEaten).getTime());
          allRows.forEach((r, index) => (r.rowNumber = index + 1));
          return allRows;
        })
      )
      .subscribe({
        next: (allRows: ReportRow[]) => {
          this.isLoading = false;
          this.downloadReport(allRows, format);
        },
        error: (err) => {
          console.error('Report generation error:', err);
          this.isLoading = false;
          this.errorMessage = 'Failed to generate report. Please try again later.';
        }
      });
  }

  private downloadReport(allRows: ReportRow[], format: 'csv' | 'xlsx') {
    const nutrientColumns = this.trackedNutrients.map(tn => {
      const [name, unit] = this.getNutrientLabelPair(tn.nutrientId);
      return unit ? `${name} (${unit})` : name;
    });
    const header = ['#', 'Date', 'Item Name', ...nutrientColumns];
    const dataRows = allRows.map(r => {
      const dateObj = new Date(r.dateEaten);
      const formattedDate = dateObj.toISOString().split('T')[0];
      const nutrientValuesInOrder = this.trackedNutrients.map(tn => r.nutrients[tn.nutrientId] ?? 0);
      return [r.rowNumber, formattedDate, r.itemName, ...nutrientValuesInOrder];
    });
    const finalArray: any[][] = [header, ...dataRows];
    if (format === 'csv') {
      const csvString = Papa.unparse(finalArray);
      const blob = new Blob([csvString], { type: 'text/csv;charset=utf-8;' });
      const filename = `consumption_report_${new Date().toISOString().split('T')[0]}.csv`;
      saveAs(blob, filename);
    } else {
      const ws: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(finalArray);
      const wb: XLSX.WorkBook = { Sheets: { Report: ws }, SheetNames: ['Report'] };
      const wbout: any = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
      const blob = new Blob([wbout], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const filename = `consumption_report_${new Date().toISOString().split('T')[0]}.xlsx`;
      saveAs(blob, filename);
    }
  }
}
