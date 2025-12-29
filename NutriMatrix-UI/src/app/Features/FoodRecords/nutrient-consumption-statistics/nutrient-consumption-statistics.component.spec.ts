import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NutrientConsumptionStatisticsComponent } from './nutrient-consumption-statistics.component';

describe('NutrientConsumptionStatisticsComponent', () => {
  let component: NutrientConsumptionStatisticsComponent;
  let fixture: ComponentFixture<NutrientConsumptionStatisticsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NutrientConsumptionStatisticsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NutrientConsumptionStatisticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
