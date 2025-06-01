import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealSelectModalComponent } from './meal-select-modal.component';

describe('MealSelectModalComponent', () => {
  let component: MealSelectModalComponent;
  let fixture: ComponentFixture<MealSelectModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MealSelectModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MealSelectModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
