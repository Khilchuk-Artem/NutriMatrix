import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {environment} from '../../../../environments/environment.development';
import {HttpClient, HttpParams} from '@angular/common/http';

export interface AddFoodRecordDto {
  userId: string;
  foodMeasureId: number;
  amount: number;
  dateEaten: string; // ISO format
}

export interface UpdateFoodRecordDto {
  foodId: number;
  amountGrams: number;
  dateEaten: string;
}

export interface FoodRecordDto {
  id: number;
  userId: string;
  foodMeasureId: number;
  foodName: string;
  amount: number;
  dateEaten: string;
}

export interface NutrientInfo {
  nutrientId: number;
  name: string;
  amount: number;
  unit: string;
}


@Injectable({
  providedIn: 'root'
})
export class FoodRecordsService {
  private baseUrl = `${environment.foodRecordsApiUrl}/FoodRecord`;

  constructor(private http: HttpClient) {
  }

  addRecord(dto: AddFoodRecordDto): Observable<FoodRecordDto> {
    return this.http.post<FoodRecordDto>(this.baseUrl, dto);
  }

  deleteRecord(id: number): Observable<FoodRecordDto> {
    return this.http.delete<FoodRecordDto>(`${this.baseUrl}/${id}`);
  }

  getRecordById(id: number): Observable<FoodRecordDto> {
    return this.http.get<FoodRecordDto>(`${this.baseUrl}/${id}`);
  }

  getAllRecords(
    userId: string,
    sortByDateAsc: boolean = true,
    dateFrom?: Date,
    dateTo?: Date
  ): Observable<FoodRecordDto[]> {
    let params = new HttpParams()
      .set('userId', userId)
      .set('sortByDateAsc', sortByDateAsc.toString());

    if (dateFrom) {
      params = params.set('dateFrom', dateFrom.toISOString());
    }

    if (dateTo) {
      params = params.set('dateTo', dateTo.toISOString());
    }

    return this.http.get<FoodRecordDto[]>(this.baseUrl, {params});
  }

  updateRecord(id: number, dto: UpdateFoodRecordDto): Observable<FoodRecordDto> {
    return this.http.put<FoodRecordDto>(`${this.baseUrl}/${id}`, dto);
  }
}
