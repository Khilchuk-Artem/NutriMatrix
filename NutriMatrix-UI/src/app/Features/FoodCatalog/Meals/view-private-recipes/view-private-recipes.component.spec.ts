import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewPrivateRecipesComponent } from './view-private-recipes.component';

describe('ViewPrivateRecipesComponent', () => {
  let component: ViewPrivateRecipesComponent;
  let fixture: ComponentFixture<ViewPrivateRecipesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewPrivateRecipesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewPrivateRecipesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
