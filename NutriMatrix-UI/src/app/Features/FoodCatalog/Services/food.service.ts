import { Injectable } from '@angular/core';
import {environment} from '../../../../environments/environment.development';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {FoodDTO} from '../models/food.models';
import {FoodShortcutDTO} from '../models/food.models';

@Injectable({
  providedIn: 'root'
})
export class FoodService {
  private baseUrl = `${environment.foodCatalogApiUrl}/Food`;

  constructor(private http: HttpClient) {}

  getFoodById(id: number, includeNutrientIds?: number[]): Observable<FoodDTO> {
    let params = new HttpParams();
    if (includeNutrientIds && includeNutrientIds.length > 0) {
      includeNutrientIds.forEach(id => {
        params = params.append('includeNuntrientIds', id.toString());
      });
    }
    return this.http.get<FoodDTO>(`${this.baseUrl}/${id}`, { params });
  }

  getFoodShortcuts(
    pageNumber: number = 1,
    pageSize: number = 5,
    includeNutrientIds?: number[],
    searchQuery?: string
  ): Observable<FoodShortcutDTO[]> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (includeNutrientIds && includeNutrientIds.length > 0) {
      includeNutrientIds.forEach(id => {
        params = params.append('includeNuntrientIds', id.toString());
      });
    }

    if (searchQuery) {
      params = params.set('searchQuery', searchQuery);
    }

    return this.http.get<FoodShortcutDTO[]>(this.baseUrl, { params });
  }

  getFoodByBarcode(barcode: string, includeNutrientIds?: number[]): Observable<FoodDTO> {
    let params = new HttpParams();
    if (includeNutrientIds && includeNutrientIds.length > 0) {
      includeNutrientIds.forEach(id => {
        params = params.append('includeNutrientIds', id.toString());
      });
    }
    return this.http.get<FoodDTO>(`${this.baseUrl}/by-barcode/${barcode}`, { params });
  }
}
