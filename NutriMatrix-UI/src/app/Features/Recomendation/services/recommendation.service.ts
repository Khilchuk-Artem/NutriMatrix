import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';

// Request DTO interfaces
export interface RecipeRequestDto {
  includeIngredientIds?: string[];
  excludeIngredientIds?: string[];
  category?: string;
}

export interface RecommendationRequestDto {
  recipeRequests: RecipeRequestDto[];
  nutritionalGoals: { [key: number]: number };
}

export interface RecipeShortcutDto {
  id: number;
  name: string | null;
  photoUrl: string | null;
  totalServings:number;
}

export interface RecipeWithAmountDto {
  recipe: RecipeShortcutDto;
  amount: number;
}

export interface RecommendationResponseDto {
  recipesAndAmounts: RecipeWithAmountDto[];
  nutrients: { [key: number]: number };
  totalDistance: number;
  timeToRespondInMs: number;
}

@Injectable({
  providedIn: 'root'
})
export class RecommendationService {
  private baseUrl = `${environment.recipeApiUrl}/api/Recommendation`;

  constructor(private http: HttpClient) {}

  getRecommendation(dto: RecommendationRequestDto): Observable<RecommendationResponseDto> {
    return this.http.post<RecommendationResponseDto>(`${this.baseUrl}/get-recommendation`, dto);
  }
}
