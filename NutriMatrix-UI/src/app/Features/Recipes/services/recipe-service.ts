import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../../environments/environment.development';

export interface NutrientAmountDto {
  nutrientId: number;
  amount: number;
}

export interface IngredientMeasureDto {
  amount:number;
  foodId: number;
  measureId: number;
}

export interface RecipeShortcutDto {
  id: number;
  title: string;
  recipeId: number;
  servings: number;
  category: string;
  ingredients: IngredientMeasureDto[];  // Зверни увагу, тут список об'єктів
  nutrients: NutrientAmountDto[];
}

@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  private baseUrl = `${environment.recipeApiUrl}/api/Recipe`;

  constructor(private http: HttpClient) {}

  getShortcut(id: number, nutrientIds?: number[]): Observable<RecipeShortcutDto> {
    let params = new HttpParams();
    if (nutrientIds && nutrientIds.length) {
      params = params.set('nutrientIds', nutrientIds.join(','));
    }
    return this.http.get<RecipeShortcutDto>(`${this.baseUrl}/shortcuts/${id}`, { params })
      .pipe(catchError(this.handleError));
  }

  getShortcuts(
    category?: string,
    query?: string,
    nutrientIds?: number[],
    page: number = 1,
    pageSize: number = 10
  ): Observable<RecipeShortcutDto[]> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (category) {
      params = params.set('category', category);
    }
    if (query) {
      params = params.set('query', query);
    }
    if (nutrientIds && nutrientIds.length) {
      params = params.set('nutrientIds', nutrientIds.join(','));
    }

    return this.http.get<RecipeShortcutDto[]>(`${this.baseUrl}/shortcuts`, { params })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any): Observable<never> {
    let errorMessage = 'An error occurred. Please try again.';
    if (error.status === 404) {
      errorMessage = 'Recipe not found.';
    } else if (error.status === 400) {
      errorMessage = 'Invalid request. Please check your input.';
    }
    console.error('RecipeService error:', error);
    return throwError(() => new Error(errorMessage));
  }
}
