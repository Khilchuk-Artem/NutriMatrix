import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPrivateRecipeComponent } from './edit-private-recipe.component';

describe('EditPrivateRecipeComponent', () => {
  let component: EditPrivateRecipeComponent;
  let fixture: ComponentFixture<EditPrivateRecipeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditPrivateRecipeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditPrivateRecipeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
