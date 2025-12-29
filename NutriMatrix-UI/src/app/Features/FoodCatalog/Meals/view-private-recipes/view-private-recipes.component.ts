import {
  Component,
  OnInit,
  AfterViewInit,
  OnDestroy,
  ViewChild,
  ElementRef
} from '@angular/core';
import {FoodMealDto, MealDto, MealService} from '../../Services/meal.service';
import {MeasureService, MeasureWithFoodDto} from '../../../FoodRecords/services/measure.service';
import {forkJoin} from 'rxjs';
import {AuthService} from '../../../Auth/Services/auth.service';
import {Router, RouterLink} from '@angular/router';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';
// … other imports …

@Component({
  selector: 'app-view-private-recipes',
  standalone: true,
  imports: [
    NgClass,
    NgIf,
    NgForOf,
    FormsModule,
    RouterLink,
    /* … */],
  templateUrl: './view-private-recipes.component.html',
  styleUrls: ['./view-private-recipes.component.css']
})
export class ViewPrivateRecipesComponent
  implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('anchor', { static: false }) anchor!: ElementRef<HTMLElement>;

  meals: MealDto[] = [];
  searchQuery = '';
  pageNumber = 1;
  pageSize = 20;
  userId = '';
  expandedRows: Record<number, boolean> = {};
  ingredients: Record<number, MeasureWithFoodDto[]> = {};

  private observer!: IntersectionObserver;
  private loadingMore = false;

  constructor(
    private mealService: MealService,
    private measureService: MeasureService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.userId = this.authService.getUser()?.userId ?? '';
    this.loadMeals();
  }

  ngAfterViewInit() {
    this.observer = new IntersectionObserver(
      entries => {
        const entry = entries[0];
        if (entry.isIntersecting && !this.loadingMore) {
          this.loadMore();
        }
      },
      { root: null, threshold: 0.1 }
    );
    this.observer.observe(this.anchor.nativeElement);
  }

  ngOnDestroy() {
    this.observer.disconnect();
  }

  loadMeals() {
    this.pageNumber = 1;
    this.mealService
      .getMeals(this.userId, this.pageNumber, this.pageSize, this.searchQuery)
      .subscribe(meals => {
        this.meals = meals;
        this.ingredients = {};
      });
  }

  loadMore() {
    this.loadingMore = true;
    this.pageNumber++;
    this.mealService
      .getMeals(this.userId, this.pageNumber, this.pageSize, this.searchQuery)
      .subscribe({
        next: more => {
          this.meals = [...this.meals, ...more];
          this.loadingMore = false;
        },
        error: () => (this.loadingMore = false)
      });
  }

  onSearch() {
    this.loadMeals();
  }

  toggleRow(mealId: number) {
    this.expandedRows[mealId] = !this.expandedRows[mealId];
    if (this.expandedRows[mealId] && !this.ingredients[mealId]) {
      const meal = this.meals.find(m => m.id === mealId);
      if (meal) this.loadIngredients(meal);
    }
  }

  loadIngredients(meal: MealDto) {
    const obs = meal.foodMeals.map(fm =>
      this.measureService.getMeasureWithFood(fm.measureId)
    );
    forkJoin(obs).subscribe(measures => {
      this.ingredients[meal.id] = measures;
    });
  }

  getIngredientAmount(
    measure: MeasureWithFoodDto,
    foodMeal: FoodMealDto,
    totalServings: number
  ): number {
    return foodMeal.quantity;
  }

  onAddNew() {
    this.router.navigate(['/app/recipes/private/add']);
  }

  onViewMore(mealId: number) {
    this.router.navigate(['/app/recipes/private', mealId]);
  }
}
