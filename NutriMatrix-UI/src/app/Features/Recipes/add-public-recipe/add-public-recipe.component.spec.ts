import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddPublicRecipeComponent } from './add-public-recipe.component';

describe('AddPublicRecipeComponent', () => {
  let component: AddPublicRecipeComponent;
  let fixture: ComponentFixture<AddPublicRecipeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddPublicRecipeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddPublicRecipeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
