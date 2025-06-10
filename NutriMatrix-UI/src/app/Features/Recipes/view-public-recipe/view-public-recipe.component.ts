import {Component, OnInit} from '@angular/core';
import {FullRecipeDto, NutrientAmountDto, RecipeService} from '../services/recipe-service';
import {NutrientInfo} from '../../../Core/services/nutrient.service';
import {NUTRIENT_CATEGORIES} from '../../../Core/services/nutrient-categories';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {NgForOf, NgIf, NgStyle} from '@angular/common';
import {AuthService} from '../../Auth/Services/auth.service';
import {MeasureService} from '../../FoodRecords/services/measure.service';
import {forkJoin} from 'rxjs';
import {CreateMealDto, MealService} from '../../FoodCatalog/Services/meal.service';

interface IngredientDetail {
  foodName: string;
  measureName: string;
  amount: number;
}
interface EnhancedNutrient extends NutrientAmountDto {
  targetAmount?: number;
  percentage?: number;
}
@Component({
  selector: 'app-view-public-recipe',
  imports: [
    NgForOf,
    NgIf,
    NgStyle
  ],
  templateUrl: './view-public-recipe.component.html',
  standalone: true,
  styleUrl: './view-public-recipe.component.css'
})
export class ViewPublicRecipeComponent implements OnInit {
  recipe: FullRecipeDto | null = null;
  nutrientMetadata: NutrientInfo[] = [];
  enhancedNutrients: EnhancedNutrient[] = [];
  groupedNutrients: Record<string, EnhancedNutrient[]> = {};
  nutrientCategories = Object.entries(NUTRIENT_CATEGORIES).map(([name, ids]) => ({ name, ids }));
  steps: string[] = [];

  // ← new:
  ingredientDetails: IngredientDetail[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private recipeService: RecipeService,
    private http: HttpClient,
    private authService: AuthService,
    private measureService: MeasureService,
    private mealService:MealService
  ) {}

  ngOnInit() {
    const user = this.authService.getUser();
    const tracked = user?.nutrientsToTrack.filter(n => n.isActive) || [];
    const targetMap = new Map(tracked.map(n => [n.nutrientId, n.targetAmount]));

    this.http
      .get<NutrientInfo[]>('assets/nutrient_attributes.json')
      .subscribe(data => (this.nutrientMetadata = data));

    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.recipeService.getRecipe(id).subscribe({
      next: r => {
        this.recipe = r;
        // --- split
        this.steps = r.directions
          .split('|')
          .map(s => s.trim())
          .filter(s => !!s);
        // --- nutrients
        this.enhancedNutrients = r.nutrients
          .filter(n => tracked.some(t => t.nutrientId === n.nutrientId))
          .map(n => ({
            ...n,
            targetAmount: targetMap.get(n.nutrientId),
            percentage:
              targetMap.get(n.nutrientId)! > 0
                ? (n.amount / targetMap.get(n.nutrientId)!) * 100
                : undefined
          }));
        this.groupByCategory();

        const calls = r.ingredients.map(ing =>
          this.measureService.getMeasureWithFood(ing.measureId)
        );
        forkJoin(calls).subscribe(measures => {
          this.ingredientDetails = measures.map((m, i) => ({
            foodName: m.food.name,
            measureName: m.name,
            amount: r.ingredients[i].amount
          }));
        });
      },
      error: () => this.router.navigate(['/app/recipes/public'])
    });
  }

  groupByCategory() {
    this.groupedNutrients = {};
    for (const { name, ids } of this.nutrientCategories) {
      this.groupedNutrients[name] = this.enhancedNutrients.filter(n =>
        ids.includes(n.nutrientId)
      );
    }
  }

  hasAnyNutrients() {
    return Object.values(this.groupedNutrients).some(arr => arr.length > 0);
  }

  nutrientLabel(id: number): [string, string] {
    const m = this.nutrientMetadata.find(n => n.attr_id === id);
    return m ? [m.name, m.unit] : ['', ''];
  }

  onEdit() {
    if (this.recipe) {
      this.router.navigate(['/app/recipes/public', this.recipe.id, 'edit']);
    }
  }

  onDelete() {
    if (!this.recipe) return;
    this.recipeService.deleteRecipe(this.recipe.id).subscribe(() => {
      this.router.navigate(['/app/recipes']);
    });
  }
  saveToMeals(){
    const userId = this.authService.getUser()?.userId || '';
    const createMealDto: CreateMealDto = {
      name: this.recipe?.title||"",
      addedBy: userId,
      totalServings: this.recipe?.servings||0,
      foodMeals: this.recipe?.ingredients.map((ing: any) => ({
        id:0,
        measureId: ing.measureId,
        quantity: ing.amount
      }))||[]
    };
    this.mealService.createMeal(createMealDto).subscribe({
      next: () => {
        this.router.navigate(['/app/recipes/private']);
      },
    });
  }
  protected readonly Math = Math;
}
