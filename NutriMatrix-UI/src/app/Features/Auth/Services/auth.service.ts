import { Injectable } from '@angular/core';
import {LoginUserDTO} from '../Models/LoginUserDTO';
import {BehaviorSubject, Observable} from 'rxjs';
import {LoginResponseDTO} from '../Models/LoginResponseDTO';
import {RequestConfirmEmailDTO} from '../Models/RequestConfirmEmailDTO';
import {HttpClient, HttpParams} from '@angular/common/http';
import {RegisterUserDTO} from '../Models/RegisterUserDTO';
import {environment} from '../../../../environments/environment.development';
import {ResetPasswordDTO} from '../Models/ResetPasswordDTO';
import {UserDto} from '../Models/UserDTO';
import {CookieService} from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = `${environment.authApiUrl}/api/Auth`;
  $user = new BehaviorSubject<UserDto | undefined>(undefined);

  constructor(private http: HttpClient,
              private cookieService:CookieService) {}

  register(dto: RegisterUserDTO): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/register`, dto);
  }

  login(dto: LoginUserDTO): Observable<LoginResponseDTO> {
    return this.http.post<LoginResponseDTO>(`${this.baseUrl}/login`, dto);
  }

  confirmEmail(dto: RequestConfirmEmailDTO): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/confirm-email`, dto, { responseType: 'text' as 'json' });
  }

  requestResetPassword(email: string): Observable<string> {
    const params = new HttpParams().set('email', email);
    return this.http.post<string>(`${this.baseUrl}/request-reset-password`, null, {
      params,
      responseType: 'text' as 'json'
    });
  }

  resetPassword(dto: ResetPasswordDTO): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/reset-password`, dto, { responseType: 'text' as 'json' });
  }

  loginViaGoogle(idToken: string): Observable<LoginResponseDTO> {
    const params = new HttpParams().set('idToken', idToken);
    return this.http.post<LoginResponseDTO>(`${this.baseUrl}/login/google`, null, { params });
  }
  setUser(response:LoginResponseDTO){
    const userDto: UserDto = {
      userId: response.userId,
      name: response.name,
      roles: response.roles,
      nutrientsToTrack: response.nutrientTrackings
    };
    this.$user.next(userDto)
    localStorage.setItem('Name', response.name)
    localStorage.setItem('Roles', response.roles.join(','))
    localStorage.setItem('UserId', response.userId)
    localStorage.setItem('NutrientsToTrack', JSON.stringify(response.nutrientTrackings));
  }
  user() : Observable<UserDto | undefined> {
    return this.$user.asObservable();
  }

  getUser(): UserDto | undefined {
    const roles = localStorage.getItem('Roles');
    const name = localStorage.getItem('Name');
    const id = localStorage.getItem('UserId');
    const nutrientsStr = localStorage.getItem('NutrientsToTrack');

    if (roles && name && id && nutrientsStr) {
      const nutrientsToTrack = JSON.parse(nutrientsStr);

      return {
        userId: id,
        name: name,
        roles: roles.split(','),
        nutrientsToTrack: nutrientsToTrack
      };
    }

    return undefined;
  }

  logout() {
    this.$user.next(undefined);
    localStorage.clear();
    this.cookieService.delete('Authorization', '/');
  }
  updateUser(summary:UserDto){
    localStorage.setItem('Name', summary.name)
    localStorage.setItem('NutrientsToTrack', JSON.stringify(summary.nutrientsToTrack));
    this.$user.next(this.getUser())
  }
}
