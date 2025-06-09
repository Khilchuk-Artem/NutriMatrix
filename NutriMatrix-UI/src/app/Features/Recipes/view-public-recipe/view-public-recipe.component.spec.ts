import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewPublicRecipeComponent } from './view-public-recipe.component';

describe('ViewPublicRecipeComponent', () => {
  let component: ViewPublicRecipeComponent;
  let fixture: ComponentFixture<ViewPublicRecipeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewPublicRecipeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewPublicRecipeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
