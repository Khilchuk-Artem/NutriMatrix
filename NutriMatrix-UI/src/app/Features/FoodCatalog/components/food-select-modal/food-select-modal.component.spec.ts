import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FoodSelectModalComponent } from './food-select-modal.component';

describe('FoodSelectModalComponent', () => {
  let component: FoodSelectModalComponent;
  let fixture: ComponentFixture<FoodSelectModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FoodSelectModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FoodSelectModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
