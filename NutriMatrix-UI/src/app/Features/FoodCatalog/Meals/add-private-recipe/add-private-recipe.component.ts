import { Component, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { FoodDTO, FoodShortcutDTO, MeasureDto } from '../../models/food.models';
import { MeasureService, MeasureWithFoodDto } from '../../../FoodRecords/services/measure.service';
import { debounceTime, filter, Subject, switchMap, takeUntil, timer } from 'rxjs';
import { FoodService } from '../../Services/food.service';
import { AuthService } from '../../../Auth/Services/auth.service';
import { CreateMealDto, MealService } from '../../Services/meal.service';
import { Router } from '@angular/router';
import { DecimalPipe, NgClass, NgForOf, NgIf, NgStyle } from '@angular/common';
import { NUTRIENT_CATEGORIES } from '../../../../Core/services/nutrient-categories';
import { HttpClient } from '@angular/common/http';
import { NgClickOutsideDirective } from 'ng-click-outside2';

interface IngredientEntry {
  measure: {
    id: number;
    name: string;
    weightInGrams: number;
    food: FoodDTO;
  };
  quantity: number;
}

interface NutrientInfo {
  attr_id: number;
  name: string;
  unit: string;
}

@Component({
  selector: 'app-add-private-recipe',
  imports: [
    ReactiveFormsModule,
    NgClass,
    NgIf,
    NgForOf,
    FormsModule,
    NgStyle,
    NgClickOutsideDirective,
    DecimalPipe
  ],
  templateUrl: './add-private-recipe.component.html',
  standalone: true,
  styleUrl: './add-private-recipe.component.css'
})
export class AddPrivateRecipeComponent implements OnInit {
  recipeForm: FormGroup;
  searchControl = new FormControl('');
  quantityControl = new FormControl('', [Validators.required, Validators.min(0.01)]);
  foodSearchResults: FoodShortcutDTO[] = [];
  selectedFood: FoodShortcutDTO | null = null;
  measuresForSelectedFood: MeasureDto[] = [];
  selectedMeasure: MeasureDto | null = null;
  addedIngredients: IngredientEntry[] = [];
  private searchSubject = new Subject<string>();
  errorMessage: string | null = null;
  nutrientMetadata: NutrientInfo[] = [];
  private consumedNutrients: Map<number, number> = new Map();
  editingIndex: number | null = null;
  editQuantityControl = new FormControl('', [Validators.required, Validators.min(0.01)]);
  editMeasure: MeasureDto | null = null;
  editMeasures: MeasureDto[] = [];
  hoveredFoodId: number | null = null;
  hoveredFood: FoodShortcutDTO | null = null;
  tooltipPosition = { x: 0, y: 0 };
  private hoverSubject = new Subject<number | null>();
  private hoverCancel$ = new Subject<void>();

  // New properties for nutrient search
  nutrientSearchControl = new FormControl('');
  nutrientSearchResults: NutrientInfo[] = [];
  selectedNutrient: NutrientInfo | null = null;
  private nutrientSearchSubject = new Subject<string>();
  hoveredNutrientId: number | null = null;
  hoveredNutrient: NutrientInfo | null = null;
  nutrientTooltipPosition = { x: 0, y: 0 };
  private nutrientHoverSubject = new Subject<number | null>();
  private nutrientHoverCancel$ = new Subject<void>();

  constructor(
    private foodService: FoodService,
    private measureService: MeasureService,
    private mealService: MealService,
    private authService: AuthService,
    private router: Router,
    private http: HttpClient
  ) {
    this.recipeForm = new FormGroup({
      name: new FormControl('', [Validators.required]),
      totalServings: new FormControl(1, [Validators.required, Validators.min(1)]),
      ingredients: new FormArray([], Validators.required)
    });
  }

  ngOnInit() {
    const trackedNutrients = this.authService.getUser()?.nutrientsToTrack?.filter(n => n.isActive).map(k => k.nutrientId) || [];

    // Food search setup
    this.searchSubject
      .pipe(
        debounceTime(300),
        filter(query => query.length > 0),
        switchMap(query => this.foodService.getFoodShortcuts(1, 10, trackedNutrients, query))
      )
      .subscribe(foods => {
        this.foodSearchResults = foods;
      });

    this.searchControl.valueChanges.subscribe(value => {
      this.searchSubject.next(value || '');
    });

    // Load nutrient metadata and setup nutrient search
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json')
      .subscribe(data => {
        this.nutrientMetadata = data;
        this.setupNutrientSearch();
      });

    // Food hover setup
    this.hoverSubject.pipe(
      switchMap(foodId => {
        if (foodId === null) {
          this.hoveredFoodId = null;
          this.hoveredFood = null;
          return [];
        }
        return timer(500).pipe(
          takeUntil(this.hoverCancel$),
          switchMap(() => {
            this.hoveredFoodId = foodId;
            return [];
          })
        );
      })
    ).subscribe();
  }

  // New method to setup nutrient search
  private setupNutrientSearch() {
    this.nutrientSearchSubject
      .pipe(
        debounceTime(300),
        filter(query => query.length > 0),
        switchMap(query => {
          const lowercaseQuery = query.toLowerCase();
          const results = this.nutrientMetadata.filter(nutrient =>
            nutrient.name.toLowerCase().includes(lowercaseQuery)
          ).slice(0, 10); // Limit to 10 results
          return [results];
        })
      )
      .subscribe(results => {
        this.nutrientSearchResults = results;
      });

    this.nutrientSearchControl.valueChanges.subscribe(value => {
      this.nutrientSearchSubject.next(value || '');
    });

    // Nutrient hover setup
    this.nutrientHoverSubject.pipe(
      switchMap(nutrientId => {
        if (nutrientId === null) {
          this.hoveredNutrientId = null;
          this.hoveredNutrient = null;
          return [];
        }
        return timer(500).pipe(
          takeUntil(this.nutrientHoverCancel$),
          switchMap(() => {
            this.hoveredNutrientId = nutrientId;
            return [];
          })
        );
      })
    ).subscribe();
  }

  // Existing food-related methods (unchanged)
  onHoverFood(food: FoodShortcutDTO) {
    this.hoverCancel$.next();
    this.hoveredFood = food;
    this.hoverSubject.next(food.id);
  }

  onLeaveFood() {
    this.hoverCancel$.next();
    this.hoverSubject.next(null);
  }

  onMouseMove(event: MouseEvent) {
    this.tooltipPosition = { x: event.clientX + 10, y: event.clientY + 10 };
  }

  // New nutrient-related methods
  onHoverNutrient(nutrient: NutrientInfo) {
    this.nutrientHoverCancel$.next();
    this.hoveredNutrient = nutrient;
    this.nutrientHoverSubject.next(nutrient.attr_id);
  }

  onLeaveNutrient() {
    this.nutrientHoverCancel$.next();
    this.nutrientHoverSubject.next(null);
  }

  onMouseMoveNutrient(event: MouseEvent) {
    this.nutrientTooltipPosition = { x: event.clientX + 10, y: event.clientY + 10 };
  }

  selectNutrient(nutrient: NutrientInfo) {
    this.selectedNutrient = nutrient;
    this.nutrientSearchControl.setValue(nutrient.name);
    this.nutrientSearchResults = [];
  }

  clearNutrientSearch() {
    this.selectedNutrient = null;
    this.nutrientSearchResults = [];
    this.nutrientSearchControl.reset();
  }

  onClickOutsideNutrientSearch() {
    if (this.nutrientSearchResults.length > 0) {
      this.nutrientSearchResults = [];
      this.nutrientSearchControl.setValue(this.selectedNutrient?.name || '');
    }
  }

  // Remaining existing methods (unchanged)
  get name() { return this.recipeForm.get('name'); }
  get totalServings() { return this.recipeForm.get('totalServings'); }
  get ingredients() { return this.recipeForm.get('ingredients') as FormArray; }

  selectFood(food: FoodShortcutDTO) {
    this.selectedFood = food;
    this.foodService.getFoodById(food.id).subscribe(res => {
      this.measuresForSelectedFood = res.measures;
      this.selectedMeasure = null;
      this.searchControl.setValue(food.name);
      this.foodSearchResults = [];
    });
  }

  addIngredient() {
    if (this.selectedMeasure && this.quantityControl.valid && this.selectedFood) {
      const tmpQuantity = this.quantityControl.value;
      const trackedNutrients = this.authService.getUser()?.nutrientsToTrack?.filter(n => n.isActive).map(k => k.nutrientId) || [];
      this.foodService.getFoodById(this.selectedFood.id, trackedNutrients).subscribe(food => {
        const ingredient: IngredientEntry = {
          measure: {
            id: this.selectedMeasure!.id,
            name: this.selectedMeasure!.name,
            weightInGrams: this.selectedMeasure!.weightInGrams,
            food: food
          },
          quantity: Number(tmpQuantity)
        };
        this.addedIngredients.push(ingredient);
        this.ingredients.push(
          new FormGroup({
            measureId: new FormControl(this.selectedMeasure!.id, [Validators.required]),
            quantity: new FormControl(tmpQuantity, [Validators.required, Validators.min(0.01)])
          })
        );
        this.resetIngredientInputs();
      });
    }
  }

  startEditing(index: number) {
    this.editingIndex = index;
    const ingredient = this.addedIngredients[index];
    this.editQuantityControl.setValue(String(ingredient.quantity));
    this.editMeasure = { id: ingredient.measure.id, name: ingredient.measure.name, weightInGrams: ingredient.measure.weightInGrams };
    this.foodService.getFoodById(ingredient.measure.food.id).subscribe(food => {
      this.editMeasures = food.measures;
    });
  }

  saveEdit(index: number) {
    if (this.editMeasure && this.editQuantityControl.valid) {
      const trackedNutrients = this.authService.getUser()?.nutrientsToTrack?.filter(n => n.isActive).map(k => k.nutrientId) || [];
      this.foodService.getFoodById(this.addedIngredients[index].measure.food.id, trackedNutrients).subscribe(food => {
        this.addedIngredients[index] = {
          measure: {
            id: this.editMeasure!.id,
            name: this.editMeasure!.name,
            weightInGrams: this.editMeasure!.weightInGrams,
            food: food
          },
          quantity: Number(this.editQuantityControl.value)
        };
        const ingredientGroup = this.ingredients.at(index) as FormGroup;
        ingredientGroup.patchValue({
          measureId: this.editMeasure!.id,
          quantity: this.editQuantityControl.value
        });
        this.cancelEdit();
      });
    }
  }

  cancelEdit() {
    this.editingIndex = null;
    this.editQuantityControl.reset();
    this.editMeasure = null;
    this.editMeasures = [];
  }

  removeIngredient(index: number) {
    this.addedIngredients.splice(index, 1);
    this.ingredients.removeAt(index);
  }

  resetIngredientInputs() {
    this.searchControl.setValue('');
    this.selectedFood = null;
    this.measuresForSelectedFood = [];
    this.selectedMeasure = null;
    this.quantityControl.reset();
    this.foodSearchResults = [];
  }

  onSubmit() {
    if (this.recipeForm.valid && this.addedIngredients.length > 0) {
      const userId = this.authService.getUser()?.userId || '';
      const createMealDto: CreateMealDto = {
        name: this.name?.value,
        addedBy: userId,
        totalServings: this.totalServings?.value,
        foodMeals: this.ingredients.value.map((ing: any) => ({
          measureId: ing.measureId,
          quantity: ing.quantity
        }))
      };
      this.mealService.createMeal(createMealDto).subscribe({
        next: () => {
          this.router.navigate(['/app/recipes/private']);
        },
        error: err => {
          this.errorMessage = 'Failed to create recipe. Please try again.';
        }
      });
    }
  }

  clearSearch() {
    this.selectedFood = null;
    this.foodSearchResults = [];
    this.searchControl.reset();
    this.selectedMeasure = null;
  }

  private calculateConsumedNutrients(): void {
    this.consumedNutrients.clear();
    const user = this.authService.getUser();
    const trackedNutrients = user?.nutrientsToTrack?.filter(n => n.isActive) || [];
    for (const ingredient of this.addedIngredients) {
      const measure = ingredient.measure;
      const amount = ingredient.quantity;
      if (!measure.food.foodNutrients) continue;
      for (const nutrient of measure.food.foodNutrients) {
        if (!trackedNutrients.some(t => t.nutrientId === nutrient.nutrientId)) continue;
        const nutrientAmount = (nutrient.amount * amount * (measure.weightInGrams || 0)) / 100;
        const currentAmount = this.consumedNutrients.get(nutrient.nutrientId) || 0;
        this.consumedNutrients.set(nutrient.nutrientId, currentAmount + nutrientAmount);
      }
    }
  }

  getConsumedNutrients(): { nutrientId: number; consumedAmount: number; targetAmount: number; percentage: number }[] {
    this.calculateConsumedNutrients();
    const user = this.authService.getUser();
    const trackedNutrients = user?.nutrientsToTrack?.filter(n => n.isActive) || [];
    const result: { nutrientId: number; consumedAmount: number; targetAmount: number; percentage: number }[] = [];
    for (const nutrient of trackedNutrients) {
      const consumedAmount = this.consumedNutrients.get(nutrient.nutrientId) || 0;
      const targetAmount = nutrient.targetAmount;
      let percentage = 0;
      if (targetAmount > 0) {
        percentage = Math.round((consumedAmount / targetAmount) * 100);
      }
      result.push({
        nutrientId: nutrient.nutrientId,
        consumedAmount: Math.round(consumedAmount * 100) / 100,
        targetAmount: targetAmount,
        percentage: percentage
      });
    }
    return result;
  }

  getNutrientCategories() {
    return Object.entries(NUTRIENT_CATEGORIES).map(([name, ids]) => ({ name, ids }));
  }

  filterConsumedNutrientsByCategory(categoryIds: number[]): { nutrientId: number; consumedAmount: number; targetAmount: number; percentage: number }[] {
    return this.getConsumedNutrients().filter(n => categoryIds.includes(n.nutrientId));
  }

  hasConsumedNutrients(): boolean {
    return this.getConsumedNutrients().length > 0;
  }

  getNutrientLabel(id: number): [string, string] {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? [match.name, match.unit] : ['', ''];
  }

  onClickOutsideSearch() {
    if (this.foodSearchResults.length > 0) {
      this.foodSearchResults = [];
      this.searchControl.setValue(this.selectedFood?.name || '');
    }
  }

  protected readonly Math = Math;
}
