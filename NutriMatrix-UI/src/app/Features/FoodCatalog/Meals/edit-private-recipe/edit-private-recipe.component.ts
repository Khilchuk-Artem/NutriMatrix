import {Component, OnInit} from '@angular/core';
import {FormArray, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {FoodDTO, FoodShortcutDTO, MeasureDto} from '../../models/food.models';
import {debounceTime, filter, Subject, switchMap, takeUntil, timer} from 'rxjs';
import {NutrientInfo} from '../../../../Core/services/nutrient.service';
import {FoodService} from '../../Services/food.service';
import {MeasureService} from '../../../FoodRecords/services/measure.service';
import {AuthService} from '../../../Auth/Services/auth.service';
import {MealDto, MealService, UpdateMealDto} from '../../Services/meal.service';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {DecimalPipe, NgClass, NgForOf, NgIf, NgStyle} from '@angular/common';
import {NUTRIENT_CATEGORIES} from '../../../../Core/services/nutrient-categories';
import {NgClickOutsideDirective} from 'ng-click-outside2';


interface IngredientEntry {
  id: number;
  measure: {
    id: number;
    name: string;
    weightInGrams: number;
    food: FoodDTO;
  };
  quantity: number;
}

@Component({
  selector: 'app-edit-private-recipe',
  imports: [
    NgStyle,
    ReactiveFormsModule,
    NgClass,
    NgIf,
    NgClickOutsideDirective,
    DecimalPipe,
    FormsModule,
    NgForOf
  ],
  templateUrl: './edit-private-recipe.component.html',
  standalone: true,
  styleUrl: './edit-private-recipe.component.css'
})
export class EditPrivateRecipeComponent implements OnInit {
  recipeForm: FormGroup;
  mealId: number;
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

  constructor(
    private foodService: FoodService,
    private measureService: MeasureService,
    private mealService: MealService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private http: HttpClient
  ) {
    this.recipeForm = new FormGroup({
      name: new FormControl('', [Validators.required]),
      totalServings: new FormControl(1, [Validators.required, Validators.min(1)]),
      ingredients: new FormArray([]),
    });
    this.mealId = +this.route.snapshot.paramMap.get('id')!;
  }

  ngOnInit() {
    this.loadMeal();
    const trackedNutrients = this.authService.getUser()?.nutrientsToTrack?.filter(n => n.isActive).map(k => k.nutrientId) || [];

    this.searchSubject.pipe(
      debounceTime(300),
      filter(query => query.length > 0),
      switchMap(query => this.foodService.getFoodShortcuts(1, 10, trackedNutrients, query))
    ).subscribe(foods => {
      this.foodSearchResults = foods;
    });

    this.searchControl.valueChanges.subscribe(value => {
      this.searchSubject.next(value || '');
    });

    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe(data => {
      this.nutrientMetadata = data;
    });

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

  private loadMeal() {
    this.mealService.getMealById(this.mealId).subscribe({
      next: (meal: MealDto) => {
        this.recipeForm.patchValue({
          name: meal.name,
          totalServings: meal.totalServings
        });

        const trackedNutrients = this.authService.getUser()?.nutrientsToTrack?.filter(n => n.isActive).map(k => k.nutrientId) || [];
        meal.foodMeals.forEach(foodMeal => {
          this.measureService.getMeasureWithFood(foodMeal.measureId).subscribe(measureWithFood=>{
            this.foodService.getFoodById(measureWithFood.food.id,trackedNutrients).subscribe(food=>{
              const ingredient: IngredientEntry = {
                id: foodMeal.id,
                measure: {
                  id: measureWithFood.id,
                  name: measureWithFood.name,
                  weightInGrams: measureWithFood.weightInGrams,
                  food: food
                },
                quantity: foodMeal.quantity
              };
              this.addedIngredients.push(ingredient);
              this.ingredients.push(
                new FormGroup({
                  id: new FormControl(foodMeal.id),
                  measureId: new FormControl(measureWithFood.id, [Validators.required]),
                  quantity: new FormControl(foodMeal.quantity, [Validators.required, Validators.min(0.01)])
                })
              );
            })
          })
        });
      },
      error: () => {
        this.errorMessage = 'Failed to load recipe. Please try again.';
      }
    });
  }

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
    this.tooltipPosition = {
      x: event.clientX + 10,
      y: event.clientY + 10
    };
  }

  get name() {
    return this.recipeForm.get('name')!;
  }

  get totalServings() {
    return this.recipeForm.get('totalServings')!;
  }

  get ingredients() {
    return this.recipeForm.get('ingredients')! as FormArray;
  }

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
          id: 0, // New foodMeal, ID will be assigned by server
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
            id: new FormControl(0),
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
    this.editMeasure = {
      id: ingredient.measure.id,
      name: ingredient.measure.name,
      weightInGrams: ingredient.measure.weightInGrams
    };
    this.foodService.getFoodById(ingredient.measure.food.id).subscribe(food => {
      this.editMeasures = food.measures;
    });
  }

  saveEdit(index: number) {
    if (this.editMeasure && this.editQuantityControl.valid) {
      const trackedNutrients = this.authService.getUser()?.nutrientsToTrack?.filter(n => n.isActive).map(k => k.nutrientId) || [];
      this.foodService.getFoodById(this.addedIngredients[index].measure.food.id, trackedNutrients).subscribe(food => {
        this.addedIngredients[index] = {
          id: this.addedIngredients[index].id,
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
      const updateMealDto: UpdateMealDto = {
        name: this.name.value,
        addedBy: userId,
        totalServings: this.totalServings.value,
        foodMeals: this.ingredients.controls.map(control => ({
          id: control.get('id')!.value,
          measureId: control.get('measureId')!.value,
          quantity: control.get('quantity')!.value
        }))
      };
      this.mealService.updateMeal(this.mealId, updateMealDto).subscribe({
        next: () => {
          this.router.navigate(['/app/recipes/private']);
        },
        error: () => {
          this.errorMessage = 'Failed to update recipe. Please try again.';
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

        var nutrientAmount = (nutrient.amount * amount * (measure.weightInGrams || 0)) / 100;

        if(measure.name=='g'){
          nutrientAmount = (nutrient.amount * amount * (measure.weightInGrams || 0)) / 10000;
        }
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
