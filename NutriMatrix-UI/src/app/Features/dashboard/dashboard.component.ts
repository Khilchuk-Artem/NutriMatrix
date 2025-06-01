import {AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {BarcodeFormat, BrowserMultiFormatReader, IScannerControls} from '@zxing/browser';
import {DecimalPipe, NgClass, NgForOf, NgIf, NgStyle} from '@angular/common';
import {ZXingScannerModule} from '@zxing/ngx-scanner';
import {DecodeHintType, Result} from '@zxing/library';
import {Calendar} from 'primeng/calendar';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {animate, style, transition, trigger} from '@angular/animations';
import {FoodDTO, FoodShortcutDTO, MeasureDto} from '../FoodCatalog/models/food.models';
import {FoodService} from '../FoodCatalog/Services/food.service';
import {AuthService} from '../Auth/Services/auth.service';
import {NutrientInfo} from '../../Core/services/nutrient.service';
import {HttpClient} from '@angular/common/http';
import {AddFoodRecordDto, FoodRecordDto, FoodRecordsService} from '../FoodRecords/services/food-records.service';
import {MeasureService, MeasureWithFoodDto} from '../FoodRecords/services/measure.service';
import {MenuItem} from 'primeng/api';
import {Menu} from 'primeng/menu';
import {NUTRIENT_CATEGORIES} from '../../Core/services/nutrient-categories';
import {min} from 'rxjs';
import {FoodSelectModalComponent} from '../FoodCatalog/components/food-select-modal/food-select-modal.component';
import {MealSelectModalComponent} from '../FoodCatalog/components/meal-select-modal/meal-select-modal.component';
import {MealDto, MealService} from '../FoodCatalog/Services/meal.service';
import {
  AddMealRecordDto,
  MealRecordDto,
  MealRecordsService,
  UpdateMealRecordDto
} from '../FoodRecords/services/meal-records.service';

export interface FoodRecordViewModel {
  type: 'food';
  record: FoodRecordDto;
  measure?: MeasureWithFoodDto;
}

export interface NutrientAmountDto {
  nutrientId: number;
  amount: number;
}

export interface RecipeShortcutDto {
  id: number;
  title: string;
  recipeId: number;
  servings: number;
  category: string;
  ingredientIds: number[];
  nutrients: NutrientAmountDto[];
}

export interface RecipeMeasure {
  ingredientId: number;
  quantity: number;
  measureName: string;
}

export interface Recipe {
  id: number;
  title: string;
  servings: number;
  category: string;
  measures: RecipeMeasure[];
  nutrientsPerServing: NutrientAmountDto[];
  isDeleted: boolean;
}
export interface MealRecordViewModel {
  type: 'meal';
  record: MealRecordDto;
  mealName: string;
  expanded: boolean;
  ingredientDetails: { foodName: string; measureName: string; amount: number; measure?: MeasureWithFoodDto }[];
}

type RecordViewModel = FoodRecordViewModel | MealRecordViewModel;

@Component({
  selector: 'app-dashboard',
  imports: [
    NgIf,
    ZXingScannerModule,
    NgForOf,
    Calendar,
    FormsModule,
    ReactiveFormsModule,
    Menu,
    NgClass,
    NgStyle,
    FoodSelectModalComponent,
    MealSelectModalComponent,
    DecimalPipe
  ],
  templateUrl: './dashboard.component.html',
  standalone: true,
  styleUrl: './dashboard.component.css',
  animations: [
    trigger('fadeInOut', [
      transition(':enter', [
        style({ opacity: 0 }),
        animate('150ms ease-out', style({ opacity: 1 })),
      ]),
      transition(':leave', [
        animate('150ms ease-in', style({ opacity: 0 })),
      ]),
    ]),
    trigger('scaleFade', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(0.9)' }),
        animate('300ms cubic-bezier(0.4, 0, 0.2, 1)', style({ opacity: 1, transform: 'scale(1)' })),
      ]),
      transition(':leave', [
        animate('300ms cubic-bezier(0.4, 0, 0.2, 1)', style({ opacity: 0, transform: 'scale(0.9)' })),
      ]),
    ]),
  ],
})
export class DashboardComponent  implements OnInit, OnDestroy {
  videoInputDevices: MediaDeviceInfo[] = [];
  private controls?: IScannerControls;
  date: Date = new Date(new Date().setHours(0, 0, 0, 0));
  allShortcuts: FoodShortcutDTO[] = [];
  public searchQuery = '';
  addFoodForm: FormGroup;
  isAddFoodModalOpen = false;
  includeIds: number[] = [];
  public nutrientMetadata: NutrientInfo[] = [];
  public recordViewModels: RecordViewModel[] = [];
  @ViewChild('foodModal') foodModal!: FoodSelectModalComponent;
  @ViewChild('mealModal') mealModal!: MealSelectModalComponent;
  @ViewChild('menu') menu!: Menu;

  constructor(
    private fb: FormBuilder,
    private foodService: FoodService,
    private authService: AuthService,
    private http: HttpClient,
    private foodRecordService: FoodRecordsService,
    private measureService: MeasureService,
    private mealRecordService: MealRecordsService,
    private mealService: MealService
  ) {
    this.addFoodForm = this.fb.group({
      foodName: ['', Validators.required],
      amount: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json')
      .subscribe(data => {
        this.nutrientMetadata = data;
      });
    const user = this.authService.getUser();
    if (user && 'nutrientsToTrack' in user) {
      this.includeIds = user.nutrientsToTrack
        .filter(rec => rec.isActive)
        .map(r => r.nutrientId);
    }
    this.loadRecords();
  }

  private loadRecords(): void {
    const userId = this.authService.getUser()?.userId || '';
    const endDate = new Date(this.date);
    endDate.setHours(23, 59, 59, 999);

    this.foodRecordService.getAllRecords(userId, true, this.date).subscribe(foodRecords => {
      const foodViewModels: FoodRecordViewModel[] = foodRecords.map(record => ({ type: 'food', record }));
      this.recordViewModels = [...foodViewModels, ...this.recordViewModels.filter(vm => vm.type === 'meal')];
      foodViewModels.forEach(vm => {
        this.measureService.getMeasureWithFood(vm.record.foodMeasureId).subscribe(measure => {
          vm.measure = measure;
        });
      });
      this.sortRecords();
    });

    this.mealRecordService.getAllRecords(userId, true, this.date, endDate).subscribe(mealRecords => {
      const mealViewModels: MealRecordViewModel[] = mealRecords.map(record => ({
        type: 'meal',
        record,
        mealName: 'Loading...',
        expanded: false,
        ingredientDetails: record.ingredientSnapshots.map(snapshot => ({
          foodName: 'Loading...',
          measureName: '',
          amount: snapshot.amount
        }))
      }));
      this.recordViewModels = [...this.recordViewModels.filter(vm => vm.type === 'food'), ...mealViewModels];
      mealViewModels.forEach(vm => {
        if (vm.record.mealId) {
          this.mealService.getMealById(vm.record.mealId).subscribe(meal => {
            vm.mealName = meal.name;
          });
        }
        this.loadIngredientDetails(vm);
      });
      this.sortRecords();
    });
  }

  private sortRecords(): void {
    this.recordViewModels.sort((a, b) => {
      const dateA = new Date(a.record.dateEaten);
      const dateB = new Date(b.record.dateEaten);
      return dateB.getTime() - dateA.getTime(); // Descending order
    });
  }

  private loadIngredientDetails(vm: MealRecordViewModel): void {
    vm.ingredientDetails.forEach((detail, index) => {
      const snapshot = vm.record.ingredientSnapshots[index];
      this.measureService.getMeasureWithFood(snapshot.foodMeasureId).subscribe(measure => {
        detail.foodName = measure.food.name;
        detail.measureName = measure.name;
        detail.measure = measure;
      });
    });
  }

  toggleMealRow(vm: RecordViewModel): void {
    if (vm.type === 'meal') {
      (vm as MealRecordViewModel).expanded = !(vm as MealRecordViewModel).expanded;
    }
  }

  onMealSelected(event: { editedMealRecord: number | null; meal: MealDto; servings: number }) {
    const snapshots = event.meal.foodMeals.map(fm => ({
      foodMeasureId: fm.measureId,
      amount: fm.quantity * (event.servings / event.meal.totalServings)
    }));

    const dto = {
      mealId: event.meal.id,
      dateEaten: this.date.toISOString(),
      servingsEaten: event.servings,
      ingredientSnapshots: snapshots
    };

    if (event.editedMealRecord == null) {
      const addDto: AddMealRecordDto = {
        ...dto,
        userId: this.authService.getUser()?.userId || ""
      };
      this.mealRecordService.addRecord(addDto).subscribe({
        next: () => this.loadRecords(),
        error: (err) => console.error('Failed to add meal record', err),
      });
    } else {
      this.mealRecordService.updateRecord(event.editedMealRecord, dto).subscribe({
        next: () => this.loadRecords(),
        error: (err) => console.error('Failed to update meal record', err),
      });
    }
  }

  getNutrientLabel(id: number): string[] {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? [match.name, match.unit] : ['', ''];
  }

  ngOnDestroy(): void {
    this.stopScanner();
  }

  stopScanner() {
    if (this.controls) {
      this.controls.stop();
    }
  }

  openAddFoodModal(vm?: FoodRecordViewModel, recordId?: number): void {
    if (vm) {
      this.foodModal.open({ amount: vm.record.amount, measure: vm.measure }, recordId);
    } else {
      this.foodModal.open();
    }
    this.isAddFoodModalOpen = true;
    this.foodService.getFoodShortcuts(1, 30).subscribe(res => {
      this.allShortcuts = res;
    });
  }

  openAddMealModal(vm?: MealRecordViewModel, recordId?: number): void {
    if (vm) {
      this.mealModal.open({ servings: vm.record.servingsEaten }, recordId);
    } else {
      this.mealModal.open();
    }
  }

  menuItems: MenuItem[] = [];

  openMenu(event: Event, vm: RecordViewModel): void {
    if (vm.type === 'food') {
      const foodVm = vm as FoodRecordViewModel;
      this.menuItems = [
        { label: 'Edit', icon: 'pi pi-pencil', command: () => this.onEditFood(foodVm) },
        { label: 'Delete', icon: 'pi pi-trash', command: () => this.onDeleteFood(foodVm) },
      ];
    } else {
      const mealVm = vm as MealRecordViewModel;
      this.menuItems = [
        { label: 'Edit', icon: 'pi pi-pencil', command: () => this.onEditMeal(mealVm) },
        { label: 'Delete', icon: 'pi pi-trash', command: () => this.onDeleteMeal(mealVm) },
      ];
    }
    this.menu.toggle(event);
  }

  onEditFood(vm: FoodRecordViewModel): void {
    this.openAddFoodModal(vm, vm.record.id);
  }

  onDeleteFood(vm: FoodRecordViewModel): void {
    if (!vm.record?.id || !confirm(`Delete food record for ${vm.measure?.food?.name || 'this item'}?`)) return;
    this.foodRecordService.deleteRecord(vm.record.id).subscribe({
      next: () => this.loadRecords(),
      error: (err) => console.error('Failed to delete food record:', err)
    });
  }

  onEditMeal(vm: MealRecordViewModel): void {
    this.openAddMealModal(vm, vm.record.id);
  }

  onDeleteMeal(vm: MealRecordViewModel): void {
    if (!vm.record?.id || !confirm(`Delete meal record for ${vm.mealName || 'this meal'}?`)) return;
    this.mealRecordService.deleteRecord(vm.record.id).subscribe({
      next: () => this.loadRecords(),
      error: (err) => console.error('Failed to delete meal record:', err)
    });
  }

  onDateChange(event: Date): void {
    this.date = new Date(event.getFullYear(), event.getMonth(), event.getDate());
    this.recordViewModels = []
    this.loadRecords();
  }

  private consumedNutrients: Map<number, number> = new Map();

  private calculateConsumedNutrients(): void {
    this.consumedNutrients.clear();
    const user = this.authService.getUser();
    const trackedNutrients = user?.nutrientsToTrack?.filter(n => this.includeIds.includes(n.nutrientId)) || [];

    for (const vm of this.recordViewModels) {
      if (vm.type === 'food') {
        const measure = vm.measure;
        const amount = vm.record.amount;
        if (!measure?.food?.nutrients) continue;
        for (const nutrient of measure.food.nutrients) {
          if (!this.includeIds.includes(nutrient.nutrientId)) continue;
          const nutrientAmount = (nutrient.amount * amount * (measure.weightInGrams || 0)) / 100;
          const currentAmount = this.consumedNutrients.get(nutrient.nutrientId) || 0;
          this.consumedNutrients.set(nutrient.nutrientId, currentAmount + nutrientAmount);
        }
      } else {
        const servingsEaten = vm.record.servingsEaten;
        for (const detail of vm.ingredientDetails) {
          const measure = detail.measure;
          const amount = detail.amount;
          if (!measure?.food?.nutrients) continue;
          for (const nutrient of measure.food.nutrients) {
            if (!this.includeIds.includes(nutrient.nutrientId)) continue;
            const nutrientAmount = (nutrient.amount * amount * (measure.weightInGrams || 0)) / 100;
            const currentAmount = this.consumedNutrients.get(nutrient.nutrientId) || 0;
            this.consumedNutrients.set(nutrient.nutrientId, currentAmount + nutrientAmount);
          }
        }
      }
    }
  }

  getConsumedNutrients(): { nutrientId: number; consumedAmount: number; targetAmount: number; percentage: number }[] {
    this.calculateConsumedNutrients();
    const user = this.authService.getUser();
    const trackedNutrients = user?.nutrientsToTrack?.filter(n => this.includeIds.includes(n.nutrientId)) || [];
    const result: { nutrientId: number; consumedAmount: number; targetAmount: number; percentage: number }[] = [];

    for (const nutrient of trackedNutrients) {
      const consumedAmount = this.consumedNutrients.get(nutrient.nutrientId) || 0;
      const targetAmount = nutrient.targetAmount;
      let percentage = 0;
      if (targetAmount > 0) {
        percentage = Math.round((consumedAmount / targetAmount) * 100);
      }
      result.push({
        nutrientId: nutrient.nutrientId,
        consumedAmount: Math.round(consumedAmount * 100) / 100,
        targetAmount: targetAmount,
        percentage: percentage,
      });
    }
    return result;
  }

  getNutrientCategories() {
    return Object.entries(NUTRIENT_CATEGORIES).map(([name, ids]) => ({ name, ids }));
  }

  filterConsumedNutrientsByCategory(categoryIds: number[]): { nutrientId: number; consumedAmount: number; targetAmount: number; percentage: number }[] {
    return this.getConsumedNutrients().filter(n => categoryIds.includes(n.nutrientId));
  }

  hasConsumedNutrients(): boolean {
    return this.getConsumedNutrients().length > 0;
  }

  onModalClosed() {}

  onFoodSelected(event: { editedFoodRecord: number | null, food: FoodDTO; amount: number; measure: MeasureDto }) {
    if (event.editedFoodRecord == null) {
      const dto: AddFoodRecordDto = {
        userId: this.authService.getUser()?.userId || '',
        foodMeasureId: event.measure.id,
        amount: event.amount,
        dateEaten: this.date.toISOString(),
      };
      this.foodRecordService.addRecord(dto).subscribe({
        next: () => this.loadRecords(),
        error: (err) => console.error('Failed to add food record', err),
      });
    } else {
      const dto = {
        foodMeasureId: event.measure.id,
        amount: event.amount,
      };
      this.foodRecordService.updateRecord(event.editedFoodRecord, dto).subscribe({
        next: () => this.loadRecords(),
        error: (err) => console.error('Failed to update food record', err),
      });
    }
  }

  protected readonly Math = Math;
  protected readonly console = console;
}
