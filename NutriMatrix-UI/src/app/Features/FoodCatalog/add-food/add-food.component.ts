import {Component, OnInit} from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder, FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
  ɵGetProperty
} from '@angular/forms';
import {NutrientInfo} from '../../../Core/services/nutrient.service';
import {HttpClient} from '@angular/common/http';
import {NUTRIENT_CATEGORIES} from '../../../Core/services/nutrient-categories';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {CreateFoodDto, FoodService} from '../Services/food.service';
import {FoodDTO} from '../models/food.models';
import {Router} from '@angular/router';

@Component({
  selector: 'app-add-food',
  imports: [
    ReactiveFormsModule,
    NgClass,
    NgIf,
    NgForOf
  ],
  templateUrl: './add-food.component.html',
  standalone: true,
  styleUrl: './add-food.component.css'
})
export class AddFoodComponent implements OnInit {
  foodForm: FormGroup;
  nutrientMetadata: NutrientInfo[] = [];
  groupedNutrients: { category: string, controls: { control: FormGroup, index: number }[] }[] = [];
  saving = false;

  constructor(private fb: FormBuilder,
              private http: HttpClient,
              private foodService:FoodService,
              private router:Router) {
    this.foodForm = this.fb.group({
      name: ['', Validators.required],
      photo: [''],
      barcode: [''],
      measures: this.fb.array([]),
      nutrients: this.fb.array([]),
      searchTerm: ['']
    });
    this.addGramMeasure();
  }

  ngOnInit(): void {
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe({
      next: (data) => {
        this.nutrientMetadata = data;
        this.initializeNutrients();
        this.groupNutrientsByCategory();
      },
      error: () => console.error('Failed to load nutrient metadata')
    });

    this.foodForm.get('searchTerm')?.valueChanges.subscribe(term => {
      this.groupNutrientsByCategory(term);
    });
  }

  private initializeNutrients(): void {
    const nutrientControls = this.nutrientMetadata.map(nutrient =>
      this.fb.group({
        nutrientId: [nutrient.attr_id],
        amount: [0]
      })
    );
    this.nutrients.clear();
    nutrientControls.forEach(control => this.nutrients.push(control));
  }

  get name() { return this.foodForm.get('name')!; }
  get measures() { return this.foodForm.get('measures') as FormArray; }
  get nutrients() { return this.foodForm.get('nutrients') as FormArray; }

  addMeasure(): void {
    this.measures.push(this.fb.group({
      name: ['', Validators.required],
      weightInGrams: [0, [Validators.required, Validators.min(0)]]
    }));
  }

  removeMeasure(index: number): void {
    this.measures.removeAt(index);
  }

  groupNutrientsByCategory(searchTerm: string = ''): void {
    const map = new Map<string, { control: FormGroup, index: number }[]>();
    this.nutrients.controls.forEach((control, index) => {
      const nutrientId = control.get('nutrientId')?.value;
      const nutrientLabel = this.getNutrientLabel(nutrientId).toLowerCase();
      const category = Object.entries(NUTRIENT_CATEGORIES).find(([_, ids]) => ids.includes(nutrientId))?.[0] || 'Other';

      if (!searchTerm || nutrientLabel.includes(searchTerm.toLowerCase())) {
        if (!map.has(category)) map.set(category, []);
        map.get(category)!.push({ control: control as FormGroup, index });
      }
    });

    const orderedCategories = Object.keys(NUTRIENT_CATEGORIES);
    this.groupedNutrients = orderedCategories
      .filter(category => map.has(category))
      .map(category => ({
        category,
        controls: map.get(category)!
      }));

    if (map.has('Other')) {
      this.groupedNutrients.push({
        category: 'Other',
        controls: map.get('Other')!
      });
    }
  }

  getNutrientLabel(id: number): string {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? match.name : `Unknown (#${id})`;
  }
  getNutrientUnits(id: number): string {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? match.unit : `Unknown (#${id})`;
  }
  onSubmit(): void {
    if (this.foodForm.invalid) return;
    this.saving = true;

    const formValue = this.foodForm.value;

    const dto: CreateFoodDto = {
      Name: formValue.name,
      Photo: formValue.photo || null,
      Barcode: formValue.barcode || null,
      Measures: formValue.measures,
      Nutrients: formValue.nutrients.map((n: any) => ({
        NutrientId: n.nutrientId,
        Amount: n.amount
      }))
    };


    this.foodService.createFood(dto).subscribe({
      next: (createdFood: FoodDTO) => {
        this.saving = false;
        this.router.navigate(['/foods', createdFood.id]);
      },
      error: err => {
        this.saving = false;
        console.error('Error creating food:', err);
      }
    });
  }
  private addGramMeasure(): void {
    const gram = this.fb.group({
      name:          ['g', [Validators.required]],
      weightInGrams: [1, [Validators.required, Validators.min(1)]]
    });

    gram.get('name')!.disable();
    gram.get('weightInGrams')!.disable();

    this.measures.push(gram);
  }


  asFormControl(ctrl: AbstractControl | null): FormControl {
    return ctrl as FormControl;
  }

}
