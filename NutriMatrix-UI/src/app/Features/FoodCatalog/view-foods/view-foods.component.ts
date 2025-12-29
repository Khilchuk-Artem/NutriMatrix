import {Component, OnInit} from '@angular/core';
import {FoodDTO, FoodNutrientIn100gDto, FoodShortcutDTO} from '../models/food.models';
import {NutrientInfo} from '../../../Core/services/nutrient.service';
import {HttpClient} from '@angular/common/http';
import {AuthService} from '../../Auth/Services/auth.service';
import {FoodService} from '../Services/food.service';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';

@Component({
  selector: 'app-view-foods',
  imports: [
    NgIf,
    FormsModule,
    NgClass,
    NgForOf,
    RouterLink
  ],
  templateUrl: './view-foods.component.html',
  standalone: true,
  styleUrl: './view-foods.component.css'
})
export class ViewFoodsComponent implements OnInit {

  searchQuery: string = '';
  allFoods: FoodShortcutDTO[] = [];
  filteredFoods: FoodShortcutDTO[] = [];
  expandedRows: { [foodId: number]: boolean } = {};
  foodDetails: { [foodId: number]: FoodDTO } = {};
  nutrientMetadata: NutrientInfo[] = [];
  includeNutrientIds: number[] = [];

  constructor(
    private foodService: FoodService,
    private authService: AuthService,
    private http: HttpClient,
    private router:Router
  ) {}

  ngOnInit(): void {
    // Set nutrient IDs to track based on user preferences
    this.includeNutrientIds = this.authService
      .getUser()?.nutrientsToTrack?.filter(n => n.isActive).map(n => n.nutrientId) || [];

    // Load nutrient metadata for mapping IDs to names/units
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe((data) => {
      this.nutrientMetadata = data;
    });

    // Load initial food list
    this.loadFoods();
  }

  loadFoods(): void {
    // Fetch up to 100 food shortcuts for performance
    this.foodService.getFoodShortcuts(1, 100, this.includeNutrientIds).subscribe((res) => {
      this.allFoods = res;
      this.filteredFoods = res;
    });
  }

  onSearch(): void {
    const query = this.searchQuery.toLowerCase();
    this.filteredFoods = this.allFoods.filter(food => food.name.toLowerCase().includes(query));
  }

  toggleRow(foodId: number): void {
    this.expandedRows[foodId] = !this.expandedRows[foodId];
    if (this.expandedRows[foodId] && !this.foodDetails[foodId]) {
      this.foodService.getFoodById(foodId, this.includeNutrientIds).subscribe((food) => {
        this.foodDetails[foodId] = food;
      });
    }
  }

  getTopNutrients(nutrients: FoodNutrientIn100gDto[]): FoodNutrientIn100gDto[] {
    return nutrients.slice(0, 10);
  }

  onViewMore(foodId: number): void {
    this.router.navigate(['/app/food',foodId])
  }

  getNutrientLabel(id: number): string[] {
    const match = this.nutrientMetadata.find((n) => n.attr_id === id);
    return match ? [match.name, match.unit] : ['', ''];
  }
}
