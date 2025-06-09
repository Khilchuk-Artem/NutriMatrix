import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateSchedulingComponent } from './create-scheduling.component';

describe('CreateSchedulingComponent', () => {
  let component: CreateSchedulingComponent;
  let fixture: ComponentFixture<CreateSchedulingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateSchedulingComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateSchedulingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
