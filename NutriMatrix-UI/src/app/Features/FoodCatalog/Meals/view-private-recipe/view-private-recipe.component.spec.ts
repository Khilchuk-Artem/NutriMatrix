import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewPrivateRecipeComponent } from './view-private-recipe.component';

describe('ViewPrivateRecipeComponent', () => {
  let component: ViewPrivateRecipeComponent;
  let fixture: ComponentFixture<ViewPrivateRecipeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewPrivateRecipeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewPrivateRecipeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
