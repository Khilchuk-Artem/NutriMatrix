import {Component, OnInit} from '@angular/core';
import {FoodMealDto, MealDto, MealService} from '../../Services/meal.service';
import {MeasureService, MeasureWithFoodDto} from '../../../FoodRecords/services/measure.service';
import {NutrientInfo} from '../../../../Core/services/nutrient.service';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '../../../Auth/Services/auth.service';
import {HttpClient} from '@angular/common/http';
import {forkJoin, Observable} from 'rxjs';
import {NUTRIENT_CATEGORIES} from '../../../../Core/services/nutrient-categories';
import {NgForOf, NgIf, NgStyle} from '@angular/common';
import {ToastrService} from 'ngx-toastr';

interface NutrientPerServing {
  nutrientId: number;
  amount: number;
  targetAmount: number;
  percentage: number;
}

@Component({
  selector: 'app-view-private-recipe',
  imports: [
    NgIf,
    NgForOf,
    NgStyle
  ],
  templateUrl: './view-private-recipe.component.html',
  standalone: true,
  styleUrl: './view-private-recipe.component.css'
})
export class ViewPrivateRecipeComponent implements OnInit {
  meal: MealDto | null = null;
  ingredients: MeasureWithFoodDto[] = [];
  nutrientMetadata: NutrientInfo[] = [];
  consumedNutrients: Map<number, number> = new Map(); // nutrientId -> amount per serving
  userId: string = ''; // From AuthService
  includeIds: number[] = [];

  constructor(
    private route: ActivatedRoute,
    private mealService: MealService,
    private measureService: MeasureService,
    private authService: AuthService,
    private http: HttpClient,
    private router:Router,
    private toastr:ToastrService
  ) {}

  ngOnInit(): void {
    const mealId = Number(this.route.snapshot.paramMap.get('id'));
    this.userId = this.authService.getUser()?.userId || '';

    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe(data => {
      this.nutrientMetadata = data;
    });

    const user = this.authService.getUser();
    if (user && 'nutrientsToTrack' in user) {
      this.includeIds = user.nutrientsToTrack
        .filter(rec => rec.isActive)
        .map(r => r.nutrientId);
    }

    this.mealService.getMealById(mealId).subscribe(meal => {
      this.meal = meal;
      this.loadIngredients(meal);
    });
  }

  loadIngredients(meal: MealDto): void {
    const measureObservables: Observable<MeasureWithFoodDto>[] = meal.foodMeals
      .map((fm: FoodMealDto) => this.measureService.getMeasureWithFood(fm.measureId));

    forkJoin(measureObservables).subscribe(measures => {
      this.ingredients = measures;
      this.calculateNutrientsPerServing();
    });
  }

  calculateNutrientsPerServing(): void {
    this.consumedNutrients.clear();
    if (!this.meal) return;

    for (let i = 0; i < this.meal.foodMeals.length; i++) {
      const foodMeal = this.meal.foodMeals[i];
      const measure = this.ingredients[i];
      if (!measure?.food?.nutrients) continue;

      const amountPerServing = foodMeal.quantity;
      for (const nutrient of measure.food.nutrients) {
        if (!this.includeIds.includes(nutrient.nutrientId)) continue;

        const nutrientAmount = (nutrient.amount * amountPerServing * (measure.weightInGrams || 0)) / 100;
        const currentAmount = this.consumedNutrients.get(nutrient.nutrientId) || 0;
        this.consumedNutrients.set(nutrient.nutrientId, currentAmount + nutrientAmount);
      }
    }
  }

  getNutrientsPerServing(): NutrientPerServing[] {
    const user = this.authService.getUser();
    const trackedNutrients = user?.nutrientsToTrack?.filter(n => this.includeIds.includes(n.nutrientId)) || [];
    const result: NutrientPerServing[] = [];

    for (const nutrient of trackedNutrients) {
      const amount = this.consumedNutrients.get(nutrient.nutrientId) || 0;
      const targetAmount = nutrient.targetAmount;
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

  getNutrientLabel(id: number): string[] {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? [match.name, match.unit] : ['', ''];
  }

  getNutrientCategories() {
    return Object.entries(NUTRIENT_CATEGORIES).map(([name, ids]) => ({ name, ids }));
  }

  filterNutrientsByCategory(categoryIds: number[]): NutrientPerServing[] {
    return this.getNutrientsPerServing().filter(n => categoryIds.includes(n.nutrientId));
  }

  hasNutrients(): boolean {
    return this.getNutrientsPerServing().length > 0;
  }

  protected readonly Math = Math;

  onDelete() {
    this.mealService.deleteMeal(this.meal?.id||0).subscribe(res=>{
      this.router.navigate(['/app/recipes/private']).then(()=>{
        this.toastr.success('Deleted successfully!')
      })
    })
  }

  onEdit(meal:MealDto) {
    this.router.navigate(['/app/recipes/private',meal.id,'edit']);
  }
}
