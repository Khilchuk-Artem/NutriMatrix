import { Injectable } from '@angular/core';
import {environment} from '../../../../environments/environment.development';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';

export interface FoodMealDto {
  id:number;
  measureId: number;
  quantity: number;
}

export interface UpdateFoodMealDto {
  id: number;
  measureId: number;
  quantity: number;
}

export interface MealDto {
  id: number;
  name: string;
  addedBy: string;
  totalServings: number;
  foodMeals: FoodMealDto[];
}

export interface CreateMealDto {
  name: string;
  addedBy: string;
  totalServings: number;
  foodMeals: FoodMealDto[];
}

export interface UpdateMealDto {
  name: string;
  addedBy: string;
  totalServings: number;
  foodMeals: UpdateFoodMealDto[];
}

@Injectable({
  providedIn: 'root'
})
export class MealService {
  private baseUrl = `${environment.foodCatalogApiUrl}/api/Meal`;

  constructor(private http: HttpClient) {}

  getMeals(
    userId: string,
    pageNumber: number = 1,
    pageSize: number = 20,
    searchQuery?: string
  ): Observable<MealDto[]> {
    let params = new HttpParams()
      .set('userId', userId)
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (searchQuery) {
      console.log(searchQuery)
      params = params.set('searchQuery', searchQuery);
    }

    return this.http.get<MealDto[]>(this.baseUrl, { params });
  }

  getMealById(id: number): Observable<MealDto> {
    return this.http.get<MealDto>(`${this.baseUrl}/${id}`);
  }

  createMeal(dto: CreateMealDto): Observable<MealDto> {
    return this.http.post<MealDto>(this.baseUrl, dto);
  }

  updateMeal(id: number, dto: UpdateMealDto): Observable<MealDto> {
    return this.http.put<MealDto>(`${this.baseUrl}/${id}`, dto);
  }

  deleteMeal(id: number): Observable<MealDto> {
    return this.http.delete<MealDto>(`${this.baseUrl}/${id}`);
  }
}
