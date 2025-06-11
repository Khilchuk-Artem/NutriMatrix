import {AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {RecipeService, RecipeShortcutDto} from '../services/recipe-service';
import {MenuItem} from 'primeng/api';
import {NutrientInfo} from '../../../Core/services/nutrient.service';
import {FoodService} from '../../FoodCatalog/Services/food.service';
import {AuthService} from '../../Auth/Services/auth.service';
import {HttpClient} from '@angular/common/http';
import {Router, RouterLink} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {NUTRIENT_CATEGORIES} from '../../../Core/services/nutrient-categories';
import {FormsModule} from '@angular/forms';
import {NgClass, NgForOf, NgIf, NgStyle} from '@angular/common';
import {Menu} from 'primeng/menu';
import {forkJoin, Observable} from 'rxjs';
import {FoodDTO, FoodShortcutDTO} from '../../FoodCatalog/models/food.models';
import {MeasureService, MeasureWithFoodDto} from '../../FoodRecords/services/measure.service';
import {AutoComplete} from 'primeng/autocomplete';

interface NutrientPerServing {
  nutrientId: number;
  amount: number;
  targetAmount: number;
  percentage: number;
}

@Component({
  selector: 'app-view-public-recipes',
  imports: [
    FormsModule,
    RouterLink,
    NgForOf,
    NgClass,
    Menu,
    NgIf,
    NgStyle,
    AutoComplete
  ],
  templateUrl: './view-public-recipes.component.html',
  standalone: true,
  styleUrl: './view-public-recipes.component.css'
})
export class ViewPublicRecipesComponent  implements OnInit, AfterViewInit, OnDestroy {
  recipes: RecipeShortcutDto[] = [];
  ingredients: { [key: number]: { foodName: string; quantity: number; measureName: string }[] } = {};
  expandedRows: { [key: number]: boolean } = {};
  searchQuery: string = '';
  menuItems: MenuItem[] = [];
  currentRecipeId: number | null = null;
  nutrientMetadata: NutrientInfo[] = [];
  includeIds: number[] = [];
  includedIngredients: FoodShortcutDTO[] = [];
  excludedIngredients: FoodShortcutDTO[] = [];
  ingredientsSuggestions: FoodShortcutDTO[] = [];
  isLoading: boolean = false;
  @ViewChild('anchor', { static: false }) anchor!: ElementRef<HTMLElement>;
  private observer!: IntersectionObserver;
  private page = 1;
  private pageSize = 20;
  private loadingMore = false;

  onIngredientsFilter(event: { query: string }) {
    this.foodService.getFoodShortcuts(1, 20, [], event.query).subscribe(data => {
      this.ingredientsSuggestions = data;
    });
  }

  isAdmin(){
    return this.authService.getUser()?.roles.includes('Admin');
  }
  constructor(
    private recipeService: RecipeService,
    private foodService: FoodService, // Assumed
    private authService: AuthService,
    private http: HttpClient,
    private router: Router,
    private toastr: ToastrService,
    private measureService:MeasureService
  ) {}

  ngOnInit(): void {
    // Load nutrient metadata
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe(data => {
      this.nutrientMetadata = data;
    });

    // Get tracked nutrient IDs
    const user = this.authService.getUser();
    if (user && 'nutrientsToTrack' in user) {
      this.includeIds = user.nutrientsToTrack
        .filter(rec => rec.isActive)
        .map(r => r.nutrientId);
    }

    // Load recipes
    this.loadRecipes();
  }
  ngAfterViewInit() {
    this.observer = new IntersectionObserver(entries => {
      const entry = entries[0];
      if (entry.isIntersecting && !this.loadingMore) {
        this.loadMore();
      }
    }, {
      root: null,            // viewport
      threshold: 0.1         // when 10% of sentinel is visible
    });

    this.observer.observe(this.anchor.nativeElement);
  }
  loadMore(): void {
    this.loadingMore = true;
    this.page++;
    this.recipeService.getShortcuts(
      undefined,
      this.searchQuery,
      this.includeIds,
      this.includedIngredients.map(i => i.id),
      this.excludedIngredients.map(i => i.id),
      this.page,
      this.pageSize,
    ).subscribe({
      next: recipes => {
        this.recipes = [...this.recipes, ...recipes];
        this.loadingMore = false;
      },
      error: () => {
        this.loadingMore = false;
      }
    });
  }
  ngOnDestroy() {
    this.observer.disconnect();
  }
  loadRecipes(): void {
    const includeIds = this.includedIngredients.map(i => i.id);
    const excludeIds = this.excludedIngredients.map(i => i.id);

    this.recipeService.getShortcuts(
      undefined,
      this.searchQuery,
      this.includeIds,
      includeIds,
      excludeIds,
      1,
      20,
    ).subscribe({
      next: (recipes) =>{
        this.recipes = recipes
        console.log(recipes)
      } ,
      error: (err) => this.toastr.error(err.message)
    });
  }

  toggleRow(id: number): void {
    this.expandedRows[id] = !this.expandedRows[id];
    if (this.expandedRows[id] && !this.ingredients[id]) {
      this.loadIngredients(id);
    }
  }

  loadIngredients(id: number): void {
    const recipe = this.recipes.find(r => r.id === id);
    if (!recipe) return;

    // Перевіряємо, що є ingredients
    if (!recipe.ingredients || recipe.ingredients.length === 0) {
      this.toastr.error('Invalid recipe data: ingredients missing.');
      return;
    }

    // Масив Observable для їжі
    const foodObservables: Observable<FoodDTO>[] = recipe.ingredients.map(i =>
      this.foodService.getFoodById(i.foodId)
    );

    // Масив Observable для мір
    const measureObservables: Observable<MeasureWithFoodDto>[] = recipe.ingredients.map(i =>
      this.measureService.getMeasureWithFood(i.measureId)
    );

    // Виконуємо запити паралельно
    forkJoin([...foodObservables, ...measureObservables]).subscribe({
      next: (results) => {
        const half = recipe.ingredients.length;
        const foods = results.slice(0, half) as FoodDTO[];
        const measures = results.slice(half) as MeasureWithFoodDto[];

        // Створюємо мапу foodId -> назва (для безпеки)
        const foodMap = new Map<number, string>();
        foods.forEach(f => foodMap.set(f.id, f.name));

        // Формуємо масив інгредієнтів для UI
        this.ingredients[id] = recipe.ingredients.map((ingredient, index) => {
          const measure = measures[index];
          return {
            foodName: foodMap.get(ingredient.foodId) || 'Unknown',
            quantity: ingredient.amount,
            measureName: measure ? measure.name : 'unit'
          };
        });
      },
      error: (err) => {
        this.toastr.error('Failed to load ingredients or measures: ' + err.message);
      }
    });
  }



  onViewMore(id: number): void {
    this.router.navigate(['/app/recipes/public', id]);
  }

  openMenu(event: Event, recipe: RecipeShortcutDto): void {
    this.currentRecipeId = recipe.id;
    this.menuItems = [
      {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => this.onEdit(this.currentRecipeId!)
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => this.onDelete(this.currentRecipeId!)
      }
    ];
    const menu = (event.target as HTMLElement).nextElementSibling as any;
    menu.toggle(event);
  }

  onEdit(id: number): void {
    this.router.navigate(['/app/recipes', id, 'edit']);
  }

  onDelete(id: number): void {
    // Note: RecipeService doesn't have deleteRecipe; using a placeholder
    this.toastr.error('Delete functionality not implemented in RecipeService');
    // If implemented later:
    // this.recipeService.deleteRecipe(id).subscribe({
    //   next: () => {
    //     this.recipes = this.recipes.filter(r => r.id !== id);
    //     this.toastr.success('Recipe deleted successfully!');
    //   },
    //   error: (err) => this.toastr.error(err.message)
    // });
  }

  onAddNew(): void {
    this.router.navigate(['/app/recipes/public/add']);
  }

  onSearch(): void {
    this.loadRecipes()
  }

  getNutrientLabel(id: number): string[] {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? [match.name, match.unit] : ['', ''];
  }

  getNutrientCategories() {
    return Object.entries(NUTRIENT_CATEGORIES).map(([name, ids]) => ({ name, ids }));
  }

  filterRecipeNutrientsByCategory(recipe: RecipeShortcutDto, categoryIds: number[]): NutrientPerServing[] {
    const user = this.authService.getUser();
    const trackedNutrients = user?.nutrientsToTrack?.filter(n => this.includeIds.includes(n.nutrientId)) || [];
    const result: NutrientPerServing[] = [];

    for (const nutrient of recipe.nutrients) {
      if (!categoryIds.includes(nutrient.nutrientId)) continue;
      const tracked = trackedNutrients.find(t => t.nutrientId === nutrient.nutrientId);
      if (!tracked) continue;

      const amount = nutrient.amount;
      const targetAmount = tracked.targetAmount;
      let percentage = 0;
      if (targetAmount > 0) {
        percentage = Math.round((amount / targetAmount) * 100);
      }

      result.push({
        nutrientId: nutrient.nutrientId,
        amount: Math.round(amount * 100) / 100,
        targetAmount,
        percentage
      });
    }

    return result;
  }

  protected readonly Math = Math;
}
