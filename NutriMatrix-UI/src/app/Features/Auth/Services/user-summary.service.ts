import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {UserDto} from '../Models/UserDTO';
import {HttpClient, HttpParams} from '@angular/common/http';
import {environment} from '../../../../environments/environment.development';
import {UpdateUserDTO} from '../Models/UpdateUserDTO';

@Injectable({
  providedIn: 'root'
})
export class UserSummaryService {
  private baseUrl = `${environment.authApiUrl}/api/UserSummary`;

  constructor(private http: HttpClient) {}

  getUserSummary(userId: string): Observable<UserDto> {
    const params = new HttpParams().set('userId', userId);
    return this.http.get<UserDto>(`${this.baseUrl}/me`, { params });
  }

  updateUserSummary(dto: UpdateUserDTO, userId: string): Observable<UserDto> {
    const params = new HttpParams().set('userId', userId);
    return this.http.put<UserDto>(`${this.baseUrl}/me`, dto, { params });
  }
}
