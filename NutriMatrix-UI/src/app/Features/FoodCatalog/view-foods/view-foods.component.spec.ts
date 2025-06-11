import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewFoodsComponent } from './view-foods.component';

describe('ViewFoodsComponent', () => {
  let component: ViewFoodsComponent;
  let fixture: ComponentFixture<ViewFoodsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewFoodsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewFoodsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
