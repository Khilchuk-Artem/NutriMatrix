import { Injectable } from '@angular/core';
import {environment} from '../../../../environments/environment.development';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';

export interface UpdateMealIngredientSnapshotDto{
  foodMeasureId: number;
  amount: number;
}
export interface MealIngredientSnapshotDto {
  foodMeasureId: number;
  amount: number;
}

export interface MealRecordDto {
  id: number;
  userId: string;
  mealId:number;
  dateEaten: string;
  servingsEaten: number;
  ingredientSnapshots: MealIngredientSnapshotDto[];
}

export interface AddMealRecordDto {
  userId:string;
  mealId:number;
  dateEaten: string;
  servingsEaten: number;
  ingredientSnapshots: MealIngredientSnapshotDto[];
}

export interface UpdateMealRecordDto {
  dateEaten: string;
  servingsEaten: number;
  mealId: number;
  ingredientSnapshots: UpdateMealIngredientSnapshotDto[];
}

@Injectable({
  providedIn: 'root'
})
export class MealRecordsService {
  private baseUrl = `${environment.foodRecordsApiUrl}/api/MealRecord`;

  constructor(private http: HttpClient) {}

  addRecord(dto: AddMealRecordDto): Observable<MealRecordDto> {
    return this.http.post<MealRecordDto>(this.baseUrl, dto);
  }

  deleteRecord(id: number): Observable<MealRecordDto> {
    return this.http.delete<MealRecordDto>(`${this.baseUrl}/${id}`);
  }

  getRecordById(id: number): Observable<MealRecordDto> {
    return this.http.get<MealRecordDto>(`${this.baseUrl}/${id}`);
  }

  getAllRecords(
    userId: string,
    sortByDateAsc: boolean = true,
    dateFrom?: Date,
    dateTo?: Date
  ): Observable<MealRecordDto[]> {
    let params = new HttpParams()
      .set('sortByDateAsc', sortByDateAsc.toString())
      .set('userId', userId);

    if (dateFrom) {
      params = params.set('dateFrom', dateFrom.toISOString());
    }

    if (dateTo) {
      params = params.set('dateTo', dateTo.toISOString());
    }

    return this.http.get<MealRecordDto[]>(this.baseUrl, { params });
  }

  updateRecord(id: number, dto: UpdateMealRecordDto): Observable<MealRecordDto> {
    return this.http.put<MealRecordDto>(`${this.baseUrl}/${id}`, dto);
  }
}
