import {Component, OnInit} from '@angular/core';
import {FoodMealDto, MealDto, MealService} from '../../Services/meal.service';
import {MeasureService, MeasureWithFoodDto} from '../../../FoodRecords/services/measure.service';
import {AuthService} from '../../../Auth/Services/auth.service';
import {forkJoin, Observable} from 'rxjs';
import {Panel} from 'primeng/panel';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';

@Component({
  selector: 'app-view-private-recipes',
  imports: [
    Panel,
    NgClass,
    FormsModule,
    NgIf,
    NgForOf,
    RouterLink
  ],
  templateUrl: './view-private-recipes.component.html',
  standalone: true,
  styleUrl: './view-private-recipes.component.css'
})
export class ViewPrivateRecipesComponent implements OnInit {
  meals: MealDto[] = [];
  searchQuery: string = '';
  pageNumber: number = 1;
  pageSize: number = 20;
  userId!: string;
  expandedRows: { [key: number]: boolean } = {};
  ingredients: { [key: number]: MeasureWithFoodDto[] } = {};

  constructor(
    private mealService: MealService,
    private measureService: MeasureService,
    private authService:AuthService,
    private router:Router
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUser()?.userId||""
    this.loadMeals();
  }

  loadMeals(): void {
    this.mealService.getMeals(this.userId, this.pageNumber, this.pageSize, this.searchQuery)
      .subscribe(meals => {
        this.meals = meals;
        this.ingredients = {};
      });
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.loadMeals();
  }

  toggleRow(mealId: number): void {
    this.expandedRows[mealId] = !this.expandedRows[mealId];
    if (this.expandedRows[mealId] && !this.ingredients[mealId]) {
      const meal = this.meals.find(m => m.id === mealId);
      if (meal) {
        this.loadIngredients(meal);
      }
    }
  }

  loadIngredients(meal: MealDto): void {
    const measureObservables: Observable<MeasureWithFoodDto>[] = meal.foodMeals
      .map((fm: FoodMealDto) => this.measureService.getMeasureWithFood(fm.measureId));

    forkJoin(measureObservables).subscribe(measures => {
      this.ingredients[meal.id] = measures;
    });
  }

  getIngredientAmount(measure: MeasureWithFoodDto, foodMeal: FoodMealDto, totalServings: number): number {
    return foodMeal.quantity;
  }

  onAddNew(): void {
    console.log('Add New clicked'); // Placeholder: Implement navigation or modal
  }

  onViewMore(mealId: number): void {
    this.router.navigate(['/app/recipes/private',mealId])
  }
}
