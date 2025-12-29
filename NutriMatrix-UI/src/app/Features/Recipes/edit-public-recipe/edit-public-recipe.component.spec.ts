import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPublicRecipeComponent } from './edit-public-recipe.component';

describe('EditPublicRecipeComponent', () => {
  let component: EditPublicRecipeComponent;
  let fixture: ComponentFixture<EditPublicRecipeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditPublicRecipeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditPublicRecipeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
