import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '../../../../environments/environment.development';
import {FoodDTO} from '../../FoodCatalog/models/food.models';

export interface FoodNutrientIn100gDto {
  nutrientId: number;
  amount: number;
}

export interface FoodShortcutDTO {
  id: number;
  name: string;
  nutrients: FoodNutrientIn100gDto[];
}
export interface SearchMeasureWithFoodDto {
  id: number;
  name: string;
  weightInGrams: number;
  quantity: number;
  food: FoodDTO;
}
export interface MeasureWithFoodDto {
  id: number;
  name: string;
  weightInGrams: number;
  food: FoodShortcutDTO;
}

@Injectable({
  providedIn: 'root'
})
export class MeasureService {
  private baseUrl = `${environment.foodCatalogApiUrl}/api/Measure`;

  constructor(private http: HttpClient) {}

  getMeasureWithFood(id: number): Observable<MeasureWithFoodDto> {
    return this.http.get<MeasureWithFoodDto>(`${this.baseUrl}/${id}`);
  }
  searchMeasures(query: string): Observable<SearchMeasureWithFoodDto[]> {
    if (!query?.trim()) {
      throw new Error('Query cannot be empty.');
    }
    return this.http.post<SearchMeasureWithFoodDto[]>(`${this.baseUrl}/search`, JSON.stringify(query), {
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }
}
