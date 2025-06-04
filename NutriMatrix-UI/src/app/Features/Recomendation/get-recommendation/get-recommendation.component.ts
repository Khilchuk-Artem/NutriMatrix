import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { NgClickOutsideDirective } from 'ng-click-outside2';
import { debounceTime, filter, Subject, switchMap, forkJoin } from 'rxjs';
import { FoodShortcutDTO } from '../../FoodCatalog/models/food.models';
import { FoodService } from '../../FoodCatalog/Services/food.service';
import { AutoComplete } from 'primeng/autocomplete';
import {
  RecipeWithAmountDto, RecommendationRequestDto,
  RecommendationResponseDto,
  RecommendationService
} from '../services/recommendation.service';
import { ToastrService } from 'ngx-toastr';
import { RecipeService, RecipeShortcutDto } from '../../Recipes/services/recipe-service';
import { MeasureService, MeasureWithFoodDto } from '../../FoodRecords/services/measure.service';
import { NUTRIENT_CATEGORIES } from '../../../Core/services/nutrient-categories';
import {AddMealRecordDto, MealRecordsService} from '../../FoodRecords/services/meal-records.service';
import {CreateMealDto, FoodMealDto, MealService} from '../../FoodCatalog/Services/meal.service';
import {AuthService} from '../../Auth/Services/auth.service';

interface NutrientInfo {
  attr_id: number;
  name: string;
  unit: string;
}

interface NutrientPerRecipe {
  nutrientId: number;
  amount: number;
  targetAmount: number;
  percentage: number;
}

interface DishSlot {
  category: string | null;
  includedIngredients: FoodShortcutDTO[];
  excludedIngredients: FoodShortcutDTO[];
  expanded: boolean;
}

interface NutrientGoal {
  nutrientId: number | null;
  nutrientName: string | null;
  targetQuantity: number | null;
  nutrientUnitName: string | null;
  searchControl: FormControl;
}

@Component({
  selector: 'app-get-recommendation',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, DropdownModule, MultiSelectModule, NgClickOutsideDirective, AutoComplete],
  templateUrl: 'get-recommendation.component.html',
  styleUrls: ['get-recommendation.component.css']
})
export class GetRecommendationComponent implements OnInit {
  dishSlots: DishSlot[] = [];
  nutrientGoals: NutrientGoal[] = [];
  categories: string[] = ['Breakfast', 'Lunch', 'Dinner', 'Snack'];
  nutrientMetadata: NutrientInfo[] = [];
  nutrientSearchResults: { [key: number]: NutrientInfo[] } = {};
  private searchSubjects: { [key: number]: Subject<string> } = {};
  isLoading: boolean = false;
  recommendedRecipes: RecipeWithAmountDto[] = [];
  expandedRecommendedRows: { [key: number]: boolean } = {};
  recipeIngredients: { [key: number]: { foodName: string; quantity: number; measureName: string }[] } = {};
  totalNutrientsData: NutrientPerRecipe[] = [];

  // Tooltip-related properties
  hoveredNutrient: NutrientInfo | null = null;
  hoveredNutrientId: number | null = null;
  tooltipPosition = { x: 0, y: 0 };

  constructor(
    private http: HttpClient,
    private foodService: FoodService,
    private recommendationService: RecommendationService,
    private recipeService: RecipeService,
    private measureService: MeasureService,
    private toastr: ToastrService,
    private mealService:MealService,
    private mealRecordsService:MealRecordsService,
    private authService:AuthService
  ) {}

  ngOnInit(): void {
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe({
      next: (data) => {
        this.nutrientMetadata = data;
      },
      error: (err) => {
        console.error('Failed to load nutrient metadata:', err);
      }
    });
  }

  addDishSlot() {
    if (this.dishSlots.length < 6) {
      this.dishSlots.push({
        category: null,
        includedIngredients: [],
        excludedIngredients: [],
        expanded: false
      });
    }
  }

  toggleSlotExpansion(index: number) {
    this.dishSlots[index].expanded = !this.dishSlots[index].expanded;
  }

  addNutrientGoal() {
    const searchControl = new FormControl('');
    const index = this.nutrientGoals.length;
    this.nutrientGoals.push({
      nutrientId: null,
      nutrientName: null,
      targetQuantity: null,
      nutrientUnitName: null,
      searchControl
    });
    this.nutrientSearchResults[index] = [];
    this.searchSubjects[index] = new Subject<string>();
    this.searchSubjects[index]
      .pipe(
        debounceTime(300),
        filter(query => query.length > 0),
        switchMap(query => {
          const lowercaseQuery = query.toLowerCase();
          const results = this.nutrientMetadata.filter(nutrient =>
            nutrient.name.toLowerCase().includes(lowercaseQuery) && !this.nutrientGoals.map(ng => ng.nutrientId).includes(nutrient.attr_id)
          ).slice(0, 10);
          return [results];
        })
      )
      .subscribe(results => {
        this.nutrientSearchResults[index] = results;
      });
    searchControl.valueChanges.subscribe(value => {
      this.searchSubjects[index].next(value || '');
    });
  }

  removeNutrientGoal(index: number) {
    this.nutrientGoals.splice(index, 1);
    delete this.nutrientSearchResults[index];
    delete this.searchSubjects[index];
  }

  onNutrientSearch(index: number) {
    const goal = this.nutrientGoals[index];
    if (!goal.searchControl.value) {
      this.nutrientSearchResults[index] = [];
      goal.nutrientId = null;
      goal.nutrientName = null;
    }
  }

  selectNutrient(index: number, nutrient: NutrientInfo) {
    const goal = this.nutrientGoals[index];
    goal.nutrientId = nutrient.attr_id;
    goal.nutrientName = nutrient.name;
    goal.searchControl.setValue(nutrient.name);
    goal.nutrientUnitName = nutrient.unit;
    this.nutrientSearchResults[index] = [];
    this.hoveredNutrient = null;
    this.hoveredNutrientId = null;
  }

  clearSearch(index: number) {
    const goal = this.nutrientGoals[index];
    goal.searchControl.setValue('');
    goal.nutrientId = null;
    goal.nutrientName = null;
    goal.targetQuantity = null;
    goal.nutrientUnitName = null;
    this.nutrientSearchResults[index] = [];
    this.hoveredNutrient = null;
    this.hoveredNutrientId = null;
  }

  ingredients: FoodShortcutDTO[] = [];
  loadingIngredients = false;

  onIngredientsFilter(event: { query: string }) {
    this.foodService.getFoodShortcuts(1, 20, [], event.query).subscribe(data => {
      this.ingredients = data;
    });
  }

  onClickOutsideSearch(index: number) {
    const goal = this.nutrientGoals[index];
    if (this.nutrientSearchResults[index]?.length > 0) {
      this.nutrientSearchResults[index] = [];
      goal.searchControl.setValue(goal.nutrientName || '');
      this.hoveredNutrient = null;
      this.hoveredNutrientId = null;
    }
  }

  onHoverNutrient(nutrient: NutrientInfo) {
    this.hoveredNutrient = nutrient;
    this.hoveredNutrientId = nutrient.attr_id;
  }

  onLeaveNutrient() {
    this.hoveredNutrient = null;
    this.hoveredNutrientId = null;
  }

  onMouseMove(event: MouseEvent) {
    this.tooltipPosition = { x: event.clientX + 10, y: event.clientY + 10 };
  }

  hasIncompleteGoals(): boolean {
    return this.nutrientGoals.some(goal =>
      goal.nutrientId == null || goal.targetQuantity == null
    );
  }

  findRecipes() {
    const recipeRequests = this.dishSlots.map(slot => ({
      category: slot.category ?? undefined,
      includeIngredientIds: slot.includedIngredients.map(ing => ing.id.toString()),
      excludeIngredientIds: slot.excludedIngredients.map(ing => ing.id.toString())
    }));

    const nutritionalGoals = this.nutrientGoals
      .filter(goal => goal.nutrientId != null && goal.targetQuantity != null)
      .reduce((acc, goal) => {
        acc[goal.nutrientId!] = goal.targetQuantity!;
        return acc;
      }, {} as { [key: number]: number });

    const requestData: RecommendationRequestDto = {
      recipeRequests,
      nutritionalGoals
    };

    this.isLoading = true;
    this.recommendationService.getRecommendation(requestData).subscribe({
      next: (response: RecommendationResponseDto) => {
        this.recommendedRecipes = response.recipesAndAmounts;
        this.totalNutrientsData = Object.entries(response.nutrients).map(([nutrientIdStr, amount]) => {
          const nutrientId = Number(nutrientIdStr);
          const goal = this.nutrientGoals.find(g => g.nutrientId === nutrientId);
          const targetAmount = goal ? goal.targetQuantity||0:0;
          const percentage = targetAmount > 0 ? Math.round((amount / targetAmount) * 100) : 0;
          return { nutrientId, amount, targetAmount, percentage };
        });
        this.isLoading = false;
      },
      error: (err) => {
        this.toastr.error('Failed to get recommendations: ' + err.message);
        this.isLoading = false;
      }
    });
  }

  toggleRecommendedRow(id: number) {
    if (!this.expandedRecommendedRows[id]) {
      this.expandedRecommendedRows[id] = true;
      if (!this.recipeIngredients[id]) {
        this.loadRecipeIngredients(id);
      }
    } else {
      this.expandedRecommendedRows[id] = false;
    }
  }

  loadRecipeIngredients(recipeId: number) {
    this.recipeService.getShortcut(recipeId).subscribe({
      next: (recipeDetails: RecipeShortcutDto) => {
        const ingredientObservables = recipeDetails.ingredients.map(ingredient =>
          this.measureService.getMeasureWithFood(ingredient.measureId)
        );

        forkJoin(ingredientObservables).subscribe({
          next: (measures: MeasureWithFoodDto[]) => {
            this.recipeIngredients[recipeId] = measures.map((measure, index) => {
              const ingredient = recipeDetails.ingredients[index];
              return {
                foodName: measure.food.name,
                quantity: ingredient.amount,
                measureName: measure.name
              };
            });
          },
          error: (err) => {
            this.toastr.error('Failed to load ingredients: ' + err.message);
          }
        });
      },
      error: (err) => {
        this.toastr.error('Failed to load recipe details: ' + err.message);
      }
    });
  }

  getNutrientCategories() {
    return Object.entries(NUTRIENT_CATEGORIES).map(([name, ids]) => ({ name, ids }));
  }

  filterTotalNutrientsByCategory(categoryIds: number[]): NutrientPerRecipe[] {
    return this.totalNutrientsData.filter(n => categoryIds.includes(n.nutrientId));
  }

  getNutrientLabel(id: number): string[] {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? [match.name, match.unit] : ['', ''];
  }

  protected readonly Math = Math;

  saveRecipesAndLogMeals() {
    const userId = this.authService.getUser()?.userId||"";
    if (!userId) {
      this.toastr.error('User not logged in');
      return;
    }

    this.isLoading = true;

    const observables = this.recommendedRecipes.map(recipe => {
      return this.recipeService.getShortcut(recipe.recipe.id).pipe(
        switchMap(recipeDetails => {
          const createMealDto: CreateMealDto = {
            name: recipeDetails.title,
            addedBy: userId,
            totalServings: recipeDetails.servings,
            foodMeals: recipeDetails.ingredients.map(ing => ({
              measureId: ing.measureId,
              quantity: ing.amount
            } as FoodMealDto))
          };
          return this.mealService.createMeal(createMealDto).pipe(
            switchMap(createdMeal => {
              const servingsEaten = recipe.amount;
              const ingredientSnapshots = recipeDetails.ingredients.map(ing => ({
                foodMeasureId: ing.measureId,
                amount: ing.amount * (servingsEaten / createdMeal.totalServings)
              }));
              const addMealRecordDto: AddMealRecordDto = {
                userId,
                mealId: createdMeal.id,
                dateEaten: new Date('2025-06-01T19:00:00Z').toISOString(),
                servingsEaten,
                ingredientSnapshots
              };
              return this.mealRecordsService.addRecord(addMealRecordDto);
            })
          );
        })
      );
    });

    forkJoin(observables).subscribe({
      next: () => {
        this.toastr.success('Recipes saved and meals logged successfully');
        this.isLoading = false;
      },
      error: (err) => {
        this.toastr.error('Failed to save recipes and log meals: ' + err.message);
        this.isLoading = false;
      }
    });
  }
}
