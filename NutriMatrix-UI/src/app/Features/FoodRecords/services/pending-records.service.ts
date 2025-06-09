import { Injectable } from '@angular/core';
import {environment} from '../../../../environments/environment.development';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';


export enum ConsumableType {
  Food = 0,
  Meal = 1
}

export interface PendingAdditionDto {
  consumableType: ConsumableType;
  amount: number;
  userId: string;
  consumableId: number;
  datePending: string;
}

export interface PendingRecordDto {
  id: number;
  consumableType: ConsumableType;
  amount: number;
  userId: string;
  consumableId: number;
  datePending: string;
  isDeleted: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class PendingRecordsService {
  private baseUrl = `${environment.foodRecordsApiUrl}/api/PendingRecord`;

  constructor(private http: HttpClient) {}

  getAllRecords(userId: string, startDate?: Date, endDate?: Date): Observable<PendingRecordDto[]> {
    let params = new HttpParams().set('userId', userId);

    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }

    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }

    return this.http.get<PendingRecordDto[]>(this.baseUrl, { params });
  }


  getRecordById(id: number): Observable<PendingRecordDto> {
    return this.http.get<PendingRecordDto>(`${this.baseUrl}/${id}`);
  }

  addRecord(dto: PendingAdditionDto): Observable<PendingRecordDto> {
    return this.http.post<PendingRecordDto>(this.baseUrl, dto);
  }

  updateRecord(id: number, dto: PendingAdditionDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  deleteRecord(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  confirmRecord(id: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/confirm`, {});
  }
}
