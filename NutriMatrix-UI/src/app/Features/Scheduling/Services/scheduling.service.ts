import { Injectable } from '@angular/core';
import { ConsumableType } from '../../FoodRecords/services/pending-records.service';
import { environment } from '../../../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface FoodPlanDto {
  id: number;
  consumableId: number;
  amount: number;
  requiresConfirmation: boolean;
  name: string;
  isRecurring: boolean;
  runAtUtc?: string;
  cronExpression?: string;
  userId: string;
  consumableType: ConsumableType;
  jobKey: string;
  triggerKey: string;
  isDeleted: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class FoodPlanService {
  private baseUrl = `${environment.foodRecordsApiUrl}/api/FoodPlan`;

  constructor(private http: HttpClient) {}

  createFoodPlan(dto: FoodPlanDto): Observable<FoodPlanDto> {
    return this.http.post<FoodPlanDto>(this.baseUrl, dto);
  }

  updateFoodPlan(dto: FoodPlanDto): Observable<FoodPlanDto> {
    return this.http.put<FoodPlanDto>(`${this.baseUrl}/${dto.id}`, dto);
  }

  getFoodPlanById(id: number): Observable<FoodPlanDto> {
    return this.http.get<FoodPlanDto>(`${this.baseUrl}/${id}`);
  }

  getAllFoodPlans(
    userId: string,
    recurringFirst: boolean = false,
    searchByName?: string
  ): Observable<FoodPlanDto[]> {
    let params = new HttpParams()
      .set('userId', userId.toString())
      .set('recurringFirst', recurringFirst.toString());

    if (searchByName) {
      params = params.set('searchByName', searchByName);
    }

    return this.http.get<FoodPlanDto[]>(this.baseUrl, { params });
  }
  deleteFoodPlan(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
