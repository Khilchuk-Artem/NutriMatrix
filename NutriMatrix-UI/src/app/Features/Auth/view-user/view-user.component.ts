import {Component, OnInit} from '@angular/core';
import {AuthService} from '../Services/auth.service';
import {NutrientTracking, UserDto} from '../Models/UserDTO';
import {NgForOf, NgIf} from '@angular/common';
import {NutrientInfo} from '../../../Core/services/nutrient.service';
import {HttpClient} from '@angular/common/http';
import {NUTRIENT_CATEGORIES} from "../../../Core/services/nutrient-categories";
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-view-user',
  imports: [
    NgForOf,
    NgIf,
    FormsModule
  ],
  templateUrl: './view-user.component.html',
  standalone: true,
  styleUrl: './view-user.component.css'
})
export class ViewUserComponent implements OnInit {
  public user!: UserDto | undefined;
  public nutrientMetadata: NutrientInfo[] = [];
  private nutrientMap = new Map<number, NutrientInfo>();
  searchTerm: string = '';

  constructor(
    private authService: AuthService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getUser();

    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json')
      .subscribe({
        next: data => {
          this.nutrientMetadata = data;
          this.nutrientMap = new Map(data.map(n => [n.attr_id, n]));
        },
        error: err => {
          console.error('Failed to load nutrient metadata', err);
          this.nutrientMetadata = [];
          this.nutrientMap.clear();
        }
      });
  }

  getNutrientCategories() {
    return Object.entries(NUTRIENT_CATEGORIES).map(([name, ids]) => ({ name, ids }));
  }

  filterNutrientsByCategory(categoryIds: number[]): NutrientTracking[] {
    const nutrients = this.user?.nutrientsToTrack ?? [];
    return nutrients.filter(n =>
      categoryIds.includes(n.nutrientId) &&
      this.getNutrientLabel(n.nutrientId).toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  getNutrientLabel(id: number): string {
    const match = this.nutrientMap.get(id);
    return match ? `${match.name} (${match.unit})` : `Unknown (#${id})`;
  }

}
