import {Component, OnInit} from '@angular/core';
import {FoodDTO} from '../models/food.models';
import {NutrientInfo} from '../../../Core/services/nutrient.service';
import {ActivatedRoute} from '@angular/router';
import {FoodService} from '../Services/food.service';
import {HttpClient} from '@angular/common/http';
import {NUTRIENT_CATEGORIES} from '../../../Core/services/nutrient-categories';
import {NgForOf, NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';


interface GroupedNutrient {
  category: string;
  nutrients: { id: number; name: string; unit: string; amount: number }[];
}
@Component({
  selector: 'app-view-food',
  imports: [
    NgIf,
    NgForOf,
    FormsModule
  ],
  templateUrl: './view-food.component.html',
  standalone: true,
  styleUrl: './view-food.component.css'
})
export class ViewFoodComponent implements OnInit {
  selectedFood: FoodDTO | null = null;
  nutrientMetadata: NutrientInfo[] = [];
  groupedNutrients: GroupedNutrient[] = [];
  searchTerm: string = '';

  constructor(
    private route: ActivatedRoute,
    private foodService: FoodService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe({
      next: (data) => {
        this.nutrientMetadata = data;
        this.loadFoodDetails();
      },
      error: () => console.error('Failed to load nutrient metadata')
    });
  }

  private loadFoodDetails(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      const nutrientIds = this.nutrientMetadata.map(n=>n.attr_id)
      if (id) {
        this.foodService.getFoodById(+id,nutrientIds).subscribe({
          next: (food) => {
            this.selectedFood = food;
            this.filterNutrients();
          },
          error: () => console.error('Failed to load food details')
        });
      }
    });
  }

  filterNutrients(): void {
    console.log(this.selectedFood)
    if (!this.selectedFood || this.nutrientMetadata.length === 0) {
      this.groupedNutrients = [];
      return;
    }

    const searchLower = this.searchTerm.toLowerCase();
    const filteredNutrients = this.selectedFood.foodNutrients.filter(nutrient => {
      const meta = this.nutrientMetadata.find(m => m.attr_id === nutrient.nutrientId);
      const name = meta ? meta.name.toLowerCase() : '';
      return name.includes(searchLower);
    });
    console.log(filteredNutrients)
    const map = new Map<string, { id: number; name: string; unit: string; amount: number }[]>();
    filteredNutrients.forEach(nutrient => {
      const meta = this.nutrientMetadata.find(m => m.attr_id === nutrient.nutrientId);
      const name = meta ? meta.name : `Unknown (#${nutrient.nutrientId})`;
      const unit = meta ? meta.unit : '';
      const category = Object.entries(NUTRIENT_CATEGORIES)
        .find(([_, ids]) => ids.includes(nutrient.nutrientId))?.[0] || 'Other';

      if (!map.has(category)) map.set(category, []);
      map.get(category)!.push({ id: nutrient.nutrientId, name, unit, amount: nutrient.amount });
    });

    const orderedCategories = Object.keys(NUTRIENT_CATEGORIES);
    this.groupedNutrients = orderedCategories
      .filter(cat => map.has(cat))
      .map(cat => ({ category: cat, nutrients: map.get(cat)! }));
    if (map.has('Other')) {
      this.groupedNutrients.push({ category: 'Other', nutrients: map.get('Other')! });
    }
  }
}
