import { Injectable } from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {HttpClient} from '@angular/common/http';


export interface NutrientInfo {
  attr_id: number;
  name: string;
  unit: string;
}
@Injectable({
  providedIn: 'root'
})
export class NutrientService{
  private nutrientData: NutrientInfo[] | null = null;

  constructor(private http: HttpClient) {}

  async loadNutrientData(): Promise<void> {
    if (!this.nutrientData) {
      this.nutrientData = await firstValueFrom(
        this.http.get<NutrientInfo[]>('assets/nutrients.json')
      );
    }
  }

  async getNutrientById(attrId: number): Promise<{ name: string; unit: string } | null> {
    await this.loadNutrientData();
    if (!this.nutrientData) return null;

    const match = this.nutrientData?.find(n => n.attr_id === attrId);
    return match ? { name: match.name, unit: match.unit } : null;
  }
}
