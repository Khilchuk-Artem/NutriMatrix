import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';
import { AuthService } from '../../Auth/Services/auth.service';
import { FoodPlanDto, FoodPlanService } from '../Services/scheduling.service';
import { Router } from '@angular/router';
import {
  FoodDTO,
  FoodShortcutDTO,
  MeasureDto
} from '../../FoodCatalog/models/food.models';
import { debounceTime, distinctUntilChanged, startWith } from 'rxjs';
import cronstrue from 'cronstrue';
import { MealDto, MealService } from '../../FoodCatalog/Services/meal.service';
import { FoodService } from '../../FoodCatalog/Services/food.service';
import { ConsumableType } from '../../FoodRecords/services/pending-records.service';
import { DecimalPipe, NgClass, NgForOf, NgIf, NgStyle } from '@angular/common';
import { NgClickOutsideDirective } from 'ng-click-outside2';
import { NutrientInfo } from '../../../Core/services/nutrient.service';
import { HttpClient } from '@angular/common/http';
import { CalendarModule } from 'primeng/calendar';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-create-scheduling',
  standalone: true,
  templateUrl: './create-scheduling.component.html',
  styleUrls: ['./create-scheduling.component.css'],
  imports: [
    ReactiveFormsModule,
    FormsModule,
    CalendarModule,
    NgIf,
    NgForOf,
    NgStyle,
    NgClass,
    DecimalPipe,
    NgClickOutsideDirective
  ]
})
export class CreateSchedulingComponent {
  form: FormGroup;
  loading = false;
  searchControl = new FormControl('');
  searchResults: Array<FoodShortcutDTO | MealDto> = [];
  selectedItem: FoodShortcutDTO | MealDto | null = null;
  hoveredItem: FoodDTO | MealDto | null = null;
  tooltipPosition = { x: 0, y: 0 };
  nutrientMetadata: NutrientInfo[] = [];
  minRunDate: Date = new Date();
  daysOfWeek = [
    { label: 'Mon', value: 'MON' },
    { label: 'Tue', value: 'TUE' },
    { label: 'Wed', value: 'WED' },
    { label: 'Thu', value: 'THU' },
    { label: 'Fri', value: 'FRI' },
    { label: 'Sat', value: 'SAT' },
    { label: 'Sun', value: 'SUN' }
  ];
  selectedDays: string[] = [];
  time: Date | null = null;
  foodMeasures: MeasureDto[] = [];
  selectedMeasure: MeasureDto | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private foodPlanService: FoodPlanService,
    private foodService: FoodService,
    private mealService: MealService,
    private router: Router,
    private http: HttpClient
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      consumableType: [String(ConsumableType.Food),Validators.required],
      consumableId: [null, Validators.required],
      measureId: [null],
      quantity: [null, Validators.min(0.1)],
      amount: [null, Validators.min(0.1)],
      requiresConfirmation: [false],
      isRecurring: [false],
      runAtUtc: [null, Validators.required],
      cronExpression: ['']
    });

    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json')
      .subscribe(data => this.nutrientMetadata = data);

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(q => {
        if (!this.selectedItem) this.form.get('consumableId')!.setValue(null);
        this.onSearch(q || '');
      });

    this.form.get('isRecurring')!.valueChanges
      .pipe(startWith(this.form.get('isRecurring')!.value))
      .subscribe(isRec => {
        const runAt = this.form.get('runAtUtc')!;
        const cron = this.form.get('cronExpression')!;
        if (isRec) {
          runAt.clearValidators();
          cron.setValidators([Validators.required]);
        } else {
          cron.clearValidators();
          runAt.setValidators([Validators.required]);
        }
        runAt.updateValueAndValidity();
        cron.updateValueAndValidity();
      });

    this.form.get('consumableType')!.valueChanges
      .subscribe(type => {
        this.clearSearch();
        this.selectedItem = null;
        const m = this.form.get('measureId')!;
        const q = this.form.get('quantity')!;
        const a = this.form.get('amount')!;
        if (type === ConsumableType.Food) {
          m.setValidators([Validators.required]);
          q.setValidators([Validators.required, Validators.min(0.1)]);
          a.clearValidators();
        } else {
          m.clearValidators();
          q.clearValidators();
          a.setValidators([Validators.required, Validators.min(0.1)]);
        }
        m.updateValueAndValidity();
        q.updateValueAndValidity();
        a.updateValueAndValidity();
        this.selectedItem = null;
        this.foodMeasures = [];
        this.selectedMeasure = null;
        this.form.patchValue({ measureId: null, quantity: null, amount: null });
        this.updateControlStates();
      });

    this.form.get('measureId')!.valueChanges
      .subscribe(id => {
        this.selectedMeasure = this.foodMeasures.find(x => x.id === id) || null;
        this.updateAmount();
        this.updateControlStates();
      });

    this.form.get('quantity')!.valueChanges
      .subscribe(() => this.updateAmount());

    this.updateControlStates();
  }

  get name() { return this.form.get('name'); }
  get consumableType() { return this.form.get('consumableType'); }
  get amount() { return this.form.get('amount'); }
  get runAtUtc() { return this.form.get('runAtUtc'); }
  get cronExpression() { return this.form.get('cronExpression'); }

  get cronDescription(): string {
    const expr = this.cronExpression?.value;
    try { return expr ? cronstrue.toString(expr) : ''; }
    catch { return 'Invalid cron expression'; }
  }

  isFood(item: any): item is FoodDTO {
    return item && Array.isArray((item as any).foodNutrients);
  }

  isMeal(item: any): item is MealDto {
    return item && Array.isArray((item as any).foodMeals);
  }

  onInputSearch() {
    this.selectedItem = null;
    this.form.get('consumableId')!.setValue(null);
    this.form.get('measureId')!.setValue(null);
  }

  private onSearch(q: string) {
    if (!q) {
      this.searchResults = [];
      this.selectedItem = null;
      this.foodMeasures = [];
      return;
    }
    const uid = this.authService.getUser()?.userId || '';
    if (this.form.get('consumableType')!.value === String(ConsumableType.Food)) {
      this.foodService.getFoodShortcuts(1, 10, undefined, q)
        .subscribe(list => this.searchResults = list);
    } else {
      this.mealService.getMeals(uid, 1, 10, q)
        .subscribe(list => this.searchResults = list);
    }
  }

  clearSearch() {
    this.searchControl.setValue('');
    this.selectedItem = this.hoveredItem = null;
    this.searchResults = [];
    this.foodMeasures = [];
    this.selectedMeasure = null;
    this.form.patchValue({ consumableId: null, measureId: null, quantity: null, amount: null });
    this.updateControlStates();
  }

  selectItem(item: FoodShortcutDTO | MealDto) {
    this.searchControl.setValue(item.name);
    this.form.get('consumableId')!.setValue(item.id);
    this.searchResults = [];
    this.selectedItem = item;
    if (this.form.get('consumableType')!.value == ConsumableType.Food) {
      this.foodService.getFoodById(item.id)
        .subscribe(full => {
          this.foodMeasures = full.measures;
          this.updateControlStates();
        });
    } else {
      this.updateControlStates();
    }
  }

  updateAmount() {
    const qty = this.form.get('quantity')!.value;
    if (this.selectedMeasure && qty != null) {
      this.form.get('amount')!.setValue(qty);
    }
  }

  onHoverItem(item: FoodShortcutDTO | MealDto) {
    const tracked = this.authService.getUser()?.nutrientsToTrack
      .filter(n => n.isActive).map(n => n.nutrientId) || [];
    if (this.form.get('consumableType')!.value == ConsumableType.Food) {
      this.foodService.getFoodById(item.id, tracked)
        .subscribe(full => this.hoveredItem = full);
    } else {
      this.mealService.getMealById(item.id)
        .subscribe(full => this.hoveredItem = full);
    }
  }

  onLeaveItem() { this.hoveredItem = null; }

  onMouseMove(e: MouseEvent) {
    this.tooltipPosition = { x: e.clientX + 15, y: e.clientY + 15 };
  }

  onClickOutsideSearch() { this.searchResults = []; }

  getNutrientLabel(id: number): [string, string] {
    const m = this.nutrientMetadata.find(n => n.attr_id === id);
    return m ? [m.name, m.unit] : ['', ''];
  }

  onDayChange(day: string, ev: Event) {
    const cb = ev.target as HTMLInputElement;
    cb.checked
      ? this.selectedDays.push(day)
      : this.selectedDays = this.selectedDays.filter(d => d !== day);
    this.generateCronExpression();
  }

  generateCronExpression() {
    if (!this.time || !this.selectedDays.length) {
      this.form.get('cronExpression')!.setValue('');
      return;
    }

    const local = this.time;
    const offsetMin = -local.getTimezoneOffset();
    const offsetH   = offsetMin / 60;

    let hUtc = local.getHours() - offsetH;
    let mUtc = local.getMinutes();

    let dayShift = 0;
    if (hUtc < 0) {
      hUtc += 24;
      dayShift = -1;
    } else if (hUtc >= 24) {
      hUtc -= 24;
      dayShift = 1;
    }


    const names = ['SUN','MON','TUE','WED','THU','FRI','SAT'] as const;
    const shiftedDays = this.selectedDays.map(d => {
      const idx = names.indexOf(d as any);
      const ni  = (idx + dayShift + 7) % 7;
      return names[ni];
    });

    const expr = `0 ${mUtc} ${hUtc} ? * ${shiftedDays.join(',')}`;
    this.form.get('cronExpression')!.setValue(expr);
  }



  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    const u = this.authService.getUser()?.userId || '';
    const v = this.form.value;
    const dto: FoodPlanDto = {
      id: 0,
      userId: u,
      name: v.name,
      consumableType: Number(v.consumableType),
      consumableId: v.consumableType==ConsumableType.Food? this.selectedMeasure?.id: v.consumableId,
      amount: v.consumableType==ConsumableType.Food? v.quantity: v.amount,
      requiresConfirmation: v.requiresConfirmation,
      isRecurring: v.isRecurring,
      runAtUtc: v.isRecurring ? undefined : v.runAtUtc?.toISOString(),
      cronExpression: v.isRecurring ? v.cronExpression : undefined,
      jobKey: '',
      triggerKey: '',
      isDeleted: false
    };
    this.foodPlanService.createFoodPlan(dto).subscribe({
      next: () => { this.loading = false; this.router.navigate(['/app/scheduling']); },
      error: e => { this.loading = false; alert(e.message); }
    });
  }

  private updateControlStates() {
    const type = this.form.get('consumableType')!.value;
    const itemSelected = !!this.selectedItem;
    const measureSelected = !!this.selectedMeasure;

    if (type == ConsumableType.Food) {
      const measureIdControl = this.form.get('measureId')!;
      const quantityControl = this.form.get('quantity')!;
      const amountControl = this.form.get('amount')!;

      if (itemSelected && measureIdControl.disabled) {
        measureIdControl.enable({ emitEvent: false });
      } else if (!itemSelected && measureIdControl.enabled) {
        measureIdControl.disable({ emitEvent: false });
      }

      if (measureSelected && quantityControl.disabled) {
        quantityControl.enable({ emitEvent: false });
      } else if (!measureSelected && quantityControl.enabled) {
        quantityControl.disable({ emitEvent: false });
      }

      if (amountControl.enabled) {
        amountControl.disable({ emitEvent: false });
      }
    } else {
      const measureIdControl = this.form.get('measureId')!;
      const quantityControl = this.form.get('quantity')!;
      const amountControl = this.form.get('amount')!;

      if (measureIdControl.enabled) {
        measureIdControl.disable({ emitEvent: false });
      }

      if (quantityControl.enabled) {
        quantityControl.disable({ emitEvent: false });
      }

      if (itemSelected && amountControl.disabled) {
        amountControl.enable({ emitEvent: false });
      } else if (!itemSelected && amountControl.enabled) {
        amountControl.disable({ emitEvent: false });
      }
    }
  }

  protected readonly ConsumableType = ConsumableType;
  protected readonly String = String;
  protected readonly Date = Date;
}
