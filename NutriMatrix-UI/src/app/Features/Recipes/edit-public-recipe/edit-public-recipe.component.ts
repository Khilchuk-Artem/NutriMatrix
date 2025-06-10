import {Component, OnInit} from '@angular/core';
import {CreateOrUpdateRecipeDto, FullRecipeDto, RecipeService} from '../services/recipe-service';
import {FormArray, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {FoodDTO, FoodShortcutDTO, MeasureDto} from '../../FoodCatalog/models/food.models';
import {debounceTime, filter, forkJoin, Subject, switchMap, takeUntil, timer} from 'rxjs';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {FoodService} from '../../FoodCatalog/Services/food.service';
import {NUTRIENT_CATEGORIES} from '../../../Core/services/nutrient-categories';
import {DecimalPipe, NgClass, NgForOf, NgIf, NgStyle} from '@angular/common';
import {NgClickOutsideDirective} from 'ng-click-outside2';
import {AutoComplete} from 'primeng/autocomplete';


interface IngredientEntry {
  measure: { id: number; name: string; weightInGrams: number; food: FoodDTO };
  quantity: number;
}

interface NutrientInfo {
  attr_id: number;
  name: string;
  unit: string;
}
@Component({
  selector: 'app-edit-public-recipe',
  imports: [
    NgIf,
    ReactiveFormsModule,
    NgClass,
    NgClickOutsideDirective,
    NgStyle,
    DecimalPipe,
    FormsModule,
    NgForOf,
    AutoComplete
  ],
  templateUrl: './edit-public-recipe.component.html',
  standalone: true,
  styleUrl: './edit-public-recipe.component.css'
})
export class EditPublicRecipeComponent implements OnInit {
  recipe: FullRecipeDto | null = null;
  isLoading = true;
  errorMessage: string | null = null;
  recipeForm: FormGroup;
  searchControl = new FormControl('');
  quantityControl = new FormControl('', [Validators.required, Validators.min(0.01)]);
  foodSearchResults: FoodShortcutDTO[] = [];
  selectedFood: FoodShortcutDTO | null = null;
  measuresForSelectedFood: MeasureDto[] = [];
  selectedMeasure: MeasureDto | null = null;
  addedIngredients: IngredientEntry[] = [];
  private searchSubject = new Subject<string>();
  nutrientMetadata: NutrientInfo[] = [];
  private allNutrientIds: number[] = [];
  editingIndex: number | null = null;
  editQuantityControl = new FormControl(0, [Validators.required, Validators.min(0.01)]);
  editMeasure: MeasureDto | null = null;
  editMeasures: MeasureDto[] = [];
  hoveredFoodId: number | null = null;
  hoveredFood: FoodShortcutDTO | null = null;
  tooltipPosition = { x: 0, y: 0 };
  private hoverSubject = new Subject<number | null>();
  private hoverCancel$ = new Subject<void>();
  categories: string[] = [];
  filteredCategories: string[] = [];
  editMeasureControl = new FormControl<MeasureDto | null>(null, Validators.required);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private recipeService: RecipeService,
    private foodService: FoodService,
    private http: HttpClient
  ) {
    this.recipeForm = new FormGroup({
      title: new FormControl('', [Validators.required]),
      category: new FormControl('', [Validators.required]),
      servings: new FormControl(1, [Validators.required, Validators.min(1)]),
      description: new FormControl(''),
      directions: new FormArray([new FormControl('')]),
      photoUrl: new FormControl(''),
      ingredients: new FormArray([])
    });
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.allNutrientIds = this.getAllNutrientIds();
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe(data => this.nutrientMetadata = data);

    this.searchSubject.pipe(
      debounceTime(300),
      filter(query => query.length > 0),
      switchMap(query => this.foodService.getFoodShortcuts(1, 10, this.allNutrientIds, query))
    ).subscribe(foods => this.foodSearchResults = foods);

    this.searchControl.valueChanges.subscribe(value => this.searchSubject.next(value || ''));

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

    this.recipeService.getCategories().subscribe(res => {
      this.categories = res;
      this.filteredCategories = res;
    });

    this.recipeService.getRecipe(id).subscribe({
      next: (recipe) => {
        this.recipe = recipe;
        this.initializeForm();
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load recipe. Please try again.';
        this.isLoading = false;
      }
    });
  }

  filterCategories(event: any) {
    const query = event.query.toLowerCase();
    this.filteredCategories = this.categories.filter(
      c => c.toLowerCase().includes(query)
    );
  }

  private initializeForm(): void {
    this.recipeForm.patchValue({
      title: this.recipe!.title,
      category: this.recipe!.category,
      servings: this.recipe!.servings,
      description: this.recipe!.description,
      photoUrl: this.recipe!.photoUrl
    });

    const steps = this.recipe!.directions.split(' | ');
    const directionsArray = this.directions;
    directionsArray.clear();
    if (steps.length === 0 || steps[0] === '') {
      directionsArray.push(new FormControl(''));
    } else {
      steps.forEach(step => directionsArray.push(new FormControl(step)));
    }
    const allNutrientIds = this.getAllNutrientIds();

    const ingredientsArray = this.ingredients;
    ingredientsArray.clear();
    const foodIds = [...new Set(this.recipe!.ingredients.map(ing => ing.foodId))];
    const foodObservables = foodIds.map(id => this.foodService.getFoodById(id,allNutrientIds));
    forkJoin(foodObservables).subscribe(foods => {
      const foodMap = new Map(foods.map(food => [food.id, food]));
      console.log(foodMap)
      this.recipe!.ingredients.forEach(ing => {
        const food = foodMap.get(ing.foodId) as FoodDTO;
        console.log(food)
        if (food) {
          const measure = food.measures.find(m => m.id === ing.measureId);
          if (measure) {
            const ingredientEntry: IngredientEntry = {
              measure: { id: measure.id, name: measure.name, weightInGrams: measure.weightInGrams, food:food },
              quantity: ing.amount
            };
            this.addedIngredients.push(ingredientEntry);
            ingredientsArray.push(new FormGroup({
              foodId: new FormControl(food.id, Validators.required),
              measureId: new FormControl(measure.id, Validators.required),
              amount: new FormControl(ing.amount, [Validators.required, Validators.min(0.01)]),
            }));
          }
        }
      });
    });
  }

  private getAllNutrientIds(): number[] {
    const allIds = new Set<number>();
    for (const ids of Object.values(NUTRIENT_CATEGORIES)) {
      ids.forEach(id => allIds.add(id));
    }
    return Array.from(allIds);
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
    this.tooltipPosition = { x: event.clientX + 10, y: event.clientY + 10 };
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
      this.foodService.getFoodById(this.selectedFood.id, this.allNutrientIds).subscribe(food => {
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
            foodId: new FormControl(food.id, [Validators.required]),
            measureId: new FormControl(this.selectedMeasure!.id, [Validators.required]),
            amount: new FormControl(tmpQuantity, [Validators.required, Validators.min(0.01)])
          })
        );
        this.resetIngredientInputs();
      });
    }
  }

  startEditing(index: number) {
    this.editingIndex = index;
    const ingredient = this.addedIngredients[index];
    this.editQuantityControl.setValue(ingredient.quantity);
    this.editMeasures = ingredient.measure.food.measures;
    this.editMeasureControl.setValue(
      this.editMeasures.find(m => m.id === ingredient.measure.id) || null
    );
  }

  saveEdit(index: number) {
    if (this.editMeasureControl.valid && this.editQuantityControl.valid) {
      const selectedMeasure = this.editMeasureControl.value;
      if (selectedMeasure) {
        this.foodService.getFoodById(this.addedIngredients[index].measure.food.id, this.allNutrientIds).subscribe(food => {
          this.addedIngredients[index] = {
            measure: {
              id: selectedMeasure.id,
              name: selectedMeasure.name,
              weightInGrams: selectedMeasure.weightInGrams,
              food: food
            },
            quantity: Number(this.editQuantityControl.value)
          };
          const ingredientGroup = this.ingredients.at(index) as FormGroup;
          ingredientGroup.patchValue({
            foodId: food.id,
            measureId: selectedMeasure.id,
            amount: this.editQuantityControl.value
          });
          this.cancelEdit();
        });
      }
    }
  }

  cancelEdit() {
    this.editingIndex = null;
    this.editQuantityControl.reset();
    this.editMeasureControl.reset();
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
      const directions = this.directions.controls.map(control => control.value).join(' | ');
      const dto: CreateOrUpdateRecipeDto = {
        title: this.title.value,
        category: this.category.value,
        servings: this.servings.value,
        description: this.description.value,
        directions: directions,
        photoUrl: this.photoUrl.value,
        measures: this.ingredients.value.map((ing: any) => ({
          amount: ing.amount,
          foodId: ing.foodId,
          measureId: ing.measureId
        })),
        nutrients: this.getTotalNutrients().map(n => ({ nutrientId: n.nutrientId, amount: n.amount }))
      };
      this.recipeService.updateRecipe(this.recipe!.id, dto).subscribe({
        next: () => this.router.navigate(['/app/recipes/public', this.recipe!.id]),
        error: (err) => this.errorMessage = err.message
      });
    }
  }

  get title() { return this.recipeForm.get('title')!; }
  get category() { return this.recipeForm.get('category')!; }
  get servings() { return this.recipeForm.get('servings')!; }
  get description() { return this.recipeForm.get('description')!; }
  get directions() { return this.recipeForm.get('directions') as FormArray; }
  get photoUrl() { return this.recipeForm.get('photoUrl')!; }
  get ingredients() { return this.recipeForm.get('ingredients') as FormArray; }

  addStep() {
    this.directions.push(new FormControl(''));
  }

  removeStep(index: number) {
    if (this.directions.length > 1) {
      this.directions.removeAt(index);
    }
  }

  private calculateTotalNutrients(): Map<number, number> {
    const totalNutrients = new Map<number, number>();
    for (const ingredient of this.addedIngredients) {
      const measure = ingredient.measure;
      const amount = ingredient.quantity;
      if (!measure.food.foodNutrients) {
        console.log("NOOOOOO")
        continue
      }

      for (const nutrient of measure.food.foodNutrients) {
        const nutrientAmount = (nutrient.amount * amount * measure.weightInGrams) / 100;
        const currentAmount = totalNutrients.get(nutrient.nutrientId) || 0;
        totalNutrients.set(nutrient.nutrientId, currentAmount + nutrientAmount);
      }
    }
    console.log(totalNutrients)
    return totalNutrients;
  }

  getTotalNutrients(): { nutrientId: number; amount: number }[] {
    const totalNutrients = this.calculateTotalNutrients();
    return Array.from(totalNutrients.entries()).map(([nutrientId, amount]) => ({ nutrientId, amount }));
  }

  getNutrientCategories() {
    return Object.entries(NUTRIENT_CATEGORIES).map(([name, ids]) => ({ name, ids }));
  }

  filterNutrientsByCategory(categoryIds: number[]): { nutrientId: number; amount: number }[] {
    return this.getTotalNutrients().filter(n => categoryIds.includes(n.nutrientId));
  }

  hasNutrients(): boolean {
    return this.getTotalNutrients().length > 0;
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
}
