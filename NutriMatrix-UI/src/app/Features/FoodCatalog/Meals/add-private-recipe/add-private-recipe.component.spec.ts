import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddPrivateRecipeComponent } from './add-private-recipe.component';

describe('AddPrivateRecipeComponent', () => {
  let component: AddPrivateRecipeComponent;
  let fixture: ComponentFixture<AddPrivateRecipeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddPrivateRecipeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddPrivateRecipeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
