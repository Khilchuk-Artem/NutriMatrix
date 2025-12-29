import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {MealDto, MealService} from '../../Services/meal.service';
import {NutrientInfo} from '../../../../Core/services/nutrient.service';
import {debounceTime, Subject} from 'rxjs';
import {MeasureService} from '../../../FoodRecords/services/measure.service';
import {FoodService} from '../../Services/food.service';
import {AuthService} from '../../../Auth/Services/auth.service';
import {HttpClient} from '@angular/common/http';
import {DecimalPipe, NgForOf, NgIf} from '@angular/common';
import {FoodDTO} from '../../models/food.models';
import {FormsModule} from '@angular/forms';
import {MealRecordsService} from '../../../FoodRecords/services/meal-records.service';

interface MealShortcutDTO {
  id: number;
  name: string;
}

interface NutrientTotal {
  nutrientId: number;
  amount: number;
}


@Component({
  selector: 'app-meal-select-modal',
  imports: [
    NgForOf,
    DecimalPipe,
    FormsModule,
    NgIf
  ],
  templateUrl: './meal-select-modal.component.html',
  standalone: true,
  styleUrl: './meal-select-modal.component.css'
})
export class MealSelectModalComponent implements OnInit {
  @Output() mealSelected = new EventEmitter<{
    editedMealRecord: number | null;
    meal: MealDto;
    servings: number;
  }>();
  @Output() modalClosed = new EventEmitter<void>();
  @Input() preselectedMeal: MealDto | null = null;
  @Input() preselectedRecord: { servings: number } | null = null;

  editedRecordId: number | null = null;
  isOpen = false;
  searchQuery = '';
  allMeals: MealShortcutDTO[] = [];
  selectedMeal: MealDto | null = null;
  enteredServings: number = 1;
  nutrientMetadata: NutrientInfo[] = [];
  includeNutrientIds: number[] = [];
  private searchSubject = new Subject<string>();
  private nutrientTotals: NutrientTotal[] = [];

  constructor(
    private mealService: MealService,
    private foodService: FoodService,
    private measureService: MeasureService,
    private authService: AuthService,
    private http: HttpClient,
    private recordService:MealRecordsService
  ) {}

  ngOnInit(): void {
    this.includeNutrientIds = this.authService
      .getUser()
      ?.nutrientsToTrack
      ?.filter(n => n.isActive)
      .map(n => n.nutrientId) || [];

    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe(data => {
      this.nutrientMetadata = data;
    });

    this.searchSubject.pipe(
      debounceTime(300)
    ).subscribe(query => {
      this.updateMeals();
    });
  }

  open(preselectedRecord: { servings: number } | null = null, editedRecordId: number | null = null): void {
    this.isOpen = true;
    this.editedRecordId = editedRecordId;
    const userId = this.authService.getUser()?.userId || '';
    this.enteredServings = 1;
    this.selectedMeal = null;
    this.mealService.getMeals(userId, 1, 30).subscribe(res => {
      this.allMeals = res.map(meal => ({ id: meal.id, name: meal.name }));
      if (preselectedRecord&&editedRecordId) {
        this.recordService.getRecordById(editedRecordId).subscribe(res=>{
          this.mealService.getMealById(res.mealId).subscribe(meal=>{
            this.enteredServings = preselectedRecord.servings;
            this.selectedMeal = meal;
            this.allMeals = [{ id: this.selectedMeal.id, name: this.selectedMeal.name },...this.allMeals];
            this.calculateNutrientTotals();
          })
        })
      }
    });
  }

  close(): void {
    this.isOpen = false;
    this.selectedMeal = null;
    this.searchQuery = '';
    this.enteredServings = 1;
    this.allMeals = [];
    this.nutrientTotals = [];
    this.modalClosed.emit();
  }

  updateMeals(): void {
    const userId = this.authService.getUser()?.userId || '';
    this.mealService.getMeals(userId, 1, 30, this.searchQuery).subscribe(res => {
      this.allMeals = res.map(meal => ({ id: meal.id, name: meal.name }));
    });
  }

  selectMeal(id: number): void {
    this.mealService.getMealById(id).subscribe({
      next: (meal) => {
        this.selectedMeal = meal;
        this.enteredServings = 1;
        this.calculateNutrientTotals();
      },
      error: (err) => {
        console.error('Failed to fetch meal by id', err);
      }
    });
  }

  submitMealEntry(): void {
    if (!this.selectedMeal || !this.enteredServings || this.enteredServings <= 0) return;
    this.mealSelected.emit({
      editedMealRecord: this.editedRecordId,
      meal: this.selectedMeal,
      servings: this.enteredServings
    });
    this.close();
  }

  public calculateNutrientTotals(): void {
    if (!this.selectedMeal || !this.selectedMeal.foodMeals || this.selectedMeal.foodMeals.length === 0) {
      console.warn('No selected meal or foodMeals available');
      this.nutrientTotals = [];
      return;
    }

    const totals = new Map<number, number>();
    const promises: Promise<void>[] = [];

    this.selectedMeal.foodMeals.forEach(foodMeal => {
      if (!foodMeal.measureId) {
        console.warn(`Invalid foodMeal: missing measureId`, foodMeal);
        return;
      }

      const promise = new Promise<void>((resolve) => {
        this.measureService.getMeasureWithFood(foodMeal.measureId).subscribe({
          next: (measureWithFood) => {
            console.log(measureWithFood.food.nutrients)
            if (!measureWithFood.food?.id) {
              console.warn(`MeasureWithFood missing food ID for measure ID ${foodMeal.measureId}`, measureWithFood);
              resolve();
              return;
            }

            this.foodService.getFoodById(measureWithFood.food.id, this.includeNutrientIds).subscribe({
              next: (food: FoodDTO) => {
                console.log(food)
                if (!food.foodNutrients || food.foodNutrients.length === 0) {
                  console.warn(`No nutrients for food ID ${measureWithFood.food.id}`, food);
                }

                const weightInGrams = measureWithFood.weightInGrams || 0;
                const quantity = foodMeal.quantity;
                const servingsFactor = this.enteredServings / (this.selectedMeal?.totalServings || 1);

                food.foodNutrients?.forEach(nutrient => {
                  if (this.includeNutrientIds.includes(nutrient.nutrientId)) {
                    const amount = (nutrient.amount * quantity * weightInGrams / 100) * servingsFactor;
                    console.debug(`Nutrient ${nutrient.nutrientId}: ${amount} (amount: ${nutrient.amount}, qty: ${quantity}, weight: ${weightInGrams}, servings: ${servingsFactor})`);
                    const current = totals.get(nutrient.nutrientId) || 0;
                    totals.set(nutrient.nutrientId, current + amount);
                  }
                });
                resolve();
              },
              error: (err) => {
                console.error(`Failed to fetch food ID ${measureWithFood.food.id}`, err);
                resolve(); // Continue with other foodMeals
              }
            });
          },
          error: (err) => {
            console.error(`Failed to fetch measureWithFood for measure ID ${foodMeal.measureId}`, err);
            resolve(); // Continue with other foodMeals
          }
        });
      });
      promises.push(promise);
    });

    Promise.all(promises).then(() => {
      this.nutrientTotals = Array.from(totals.entries()).map(([nutrientId, amount]) => ({
        nutrientId,
        amount
      }));
      console.debug('Nutrient Totals:', this.nutrientTotals);
    });
  }

  getNutrientTotals(): NutrientTotal[] {
    return this.nutrientTotals;
  }

  getNutrientLabel(id: number): string[] {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? [match.name, match.unit] : ['', ''];
  }

  protected readonly Math = Math;
}
