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
  amount: number;
  foodId: number;
  measureId: number;
}

export interface RecipeShortcutDto {
  id: number;
  title: string;
  recipeId: number;
  servings: number;
  category: string;
  ingredients: IngredientMeasureDto[];
  nutrients: NutrientAmountDto[];
}
export interface FullRecipeDto {
  id: number;
  title: string;
  category: string;
  servings: number;
  description: string;
  directions: string;
  photoUrl: string;
  ingredients: IngredientMeasureDto[];
  nutrients: NutrientAmountDto[];
}
export interface CreateOrUpdateRecipeDto {
  title: string;
  category: string;
  servings: number;
  description: string;
  directions: string;
  photoUrl: string;
  measures: IngredientMeasureDto[];
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
    includeIngredients?: number[],
    excludeIngredients?: number[],
    page: number = 1,
    pageSize: number = 10
  ): Observable<RecipeShortcutDto[]> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (category) params = params.set('category', category);
    if (query) params = params.set('query', query);
    if (nutrientIds && nutrientIds.length) {
      params = params.set('nutrientIds', nutrientIds.join(','));
    }
    if (includeIngredients && includeIngredients.length) {
      params = params.set('includeIngredients', includeIngredients.join(','));
    }
    if (excludeIngredients && excludeIngredients.length) {
      params = params.set('excludeIngredients', excludeIngredients.join(','));
    }

    return this.http.get<RecipeShortcutDto[]>(`${this.baseUrl}/shortcuts`, { params })
      .pipe(catchError(this.handleError));
  }

  createRecipe(dto: CreateOrUpdateRecipeDto): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, dto)
      .pipe(catchError(this.handleError));
  }

  updateRecipe(id: number, dto: CreateOrUpdateRecipeDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto)
      .pipe(catchError(this.handleError));
  }

  deleteRecipe(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`)
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
  // in recipe-service.ts

  getRecipe(id: number, nutrientIds?: number[]): Observable<FullRecipeDto> {
    let params = new HttpParams();
    if (nutrientIds && nutrientIds.length) {
      params = params.set('nutrientIds', nutrientIds.join(','));
    }
    return this.http
      .get<FullRecipeDto>(`${this.baseUrl}/${id}`, { params })
      .pipe(catchError(this.handleError));
  }
  getCategories(minRecipeCount?: number): Observable<string[]> {
    let params = new HttpParams();
    if (minRecipeCount != null) {
      params = params.set('minRecipeCount', minRecipeCount.toString());
    }
    return this.http.get<string[]>(`${this.baseUrl}/categories`, { params })
      .pipe(catchError(this.handleError));
  }
}
