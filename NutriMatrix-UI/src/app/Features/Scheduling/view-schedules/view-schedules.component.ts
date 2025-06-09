import {Component, OnInit} from '@angular/core';
import { FoodPlanDto, FoodPlanService} from '../Services/scheduling.service';
import {ConsumableType} from '../../FoodRecords/services/pending-records.service';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';
import * as cronstrue from 'cronstrue';
import {AuthService} from '../../Auth/Services/auth.service';
import {MealDto, MealService} from '../../FoodCatalog/Services/meal.service';
import {MeasureService, MeasureWithFoodDto} from '../../FoodRecords/services/measure.service';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-view-schedules',
  imports: [
    NgForOf,
    NgIf,
    NgClass,
    FormsModule,
    RouterLink
  ],
  templateUrl: './view-schedules.component.html',
  standalone: true,
  styleUrl: './view-schedules.component.css'
})
export class ViewSchedulesComponent implements OnInit {
  allPlans: FoodPlanDto[] = [];
  filteredPlans: FoodPlanDto[] = [];
  searchQuery = '';
  expandedRows: Record<number, boolean> = {};
  consumableDetails: Record<number, { name: string, displayText: string }> = {};

  constructor(
    private foodPlanService: FoodPlanService,
    private authService: AuthService,
    private measureService: MeasureService,
    private mealService: MealService
  ) {}

  ngOnInit(): void {
    const userId = this.authService.getUser()?.userId || '';
    this.foodPlanService.getAllFoodPlans(userId, true, this.searchQuery || "").subscribe(plans => {
      this.allPlans = plans;
      this.filteredPlans = plans;
      this.loadConsumableDetails();
    });
  }

  onSearch(): void {
    const userId = this.authService.getUser()?.userId || '';
    this.foodPlanService.getAllFoodPlans(userId, true, this.searchQuery || "").subscribe(plans => {
      this.allPlans = plans;
      this.filteredPlans = plans;
      this.loadConsumableDetails();
    });
  }

  toggleRow(id: number) {
    this.expandedRows[id] = !this.expandedRows[id];
  }

  convertTo24h(timeStr: string): string {
    const [time, modifier] = timeStr.split(' ');
    let [hours, minutes] = time.split(':').map(Number);
    if (modifier === 'PM' && hours !== 12) hours += 12;
    if (modifier === 'AM' && hours === 12) hours = 0;
    return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:00`;
  }

  describeSchedule(plan: FoodPlanDto): string {
    if (plan.isRecurring && plan.cronExpression) {
      try {
        const raw = cronstrue.toString(plan.cronExpression, { locale: 'en' });
        const match = raw.match(/At (\d{1,2}:\d{2} (AM|PM)), only on (\w+)/i);
        if (!match) return raw;

        const [, timeStr, , dayStr] = match;
        const utcTime = new Date(`2000-01-01T${this.convertTo24h(timeStr)}Z`);
        const localTime = new Date(utcTime.getTime() + 3 * 60 * 60 * 1000);

        let adjustedDay = dayStr;
        if (localTime.getUTCDay() !== utcTime.getUTCDay()) {
          adjustedDay = this.getNextDay(dayStr);
        }

        const formatted = localTime.toLocaleTimeString('en-US', {
          hour: '2-digit',
          minute: '2-digit',
          hour12: true,
          timeZone: 'UTC'
        });

        return `${formatted}, only on ${adjustedDay},`;
      } catch {
        return plan.cronExpression;
      }
    }
    return `Runs at ${new Date(plan.runAtUtc!).toLocaleString(undefined, {
      timeZone: 'Europe/Istanbul'
    })}`;
  }

  getNextDay(day: string): string {
    const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    const index = days.indexOf(day);
    return index >= 0 ? days[(index + 1) % 7] : day;
  }

  deletePlan(id: number) {
    this.foodPlanService.deleteFoodPlan(id).subscribe(() => {
      window.location.reload();
    });
  }

  loadConsumableDetails(): void {
    this.allPlans.forEach(plan => {
      if (plan.consumableType === ConsumableType.Food) {
        this.measureService.getMeasureWithFood(plan.consumableId).subscribe({
          next: (measure: MeasureWithFoodDto) => {
            const displayText = `${measure.food.name} - ${plan.amount} ${measure.name}`;
            this.consumableDetails[plan.id] = { name: measure.food.name, displayText };
          },
          error: () => {
            this.consumableDetails[plan.id] = { name: 'Unknown Food', displayText: 'Error loading food details' };
          }
        });
      } else if (plan.consumableType == ConsumableType.Meal) {
        this.mealService.getMealById(plan.consumableId).subscribe({
          next: (meal: MealDto) => {
            const displayText = `${meal.name} - ${plan.amount} servings`;
            this.consumableDetails[plan.id] = { name: meal.name, displayText };
          },
          error: () => {
            this.consumableDetails[plan.id] = { name: 'Unknown Recipe', displayText: 'Error loading recipe details' };
          }
        });
      }
    });
  }

  protected readonly ConsumableType = ConsumableType;
}
