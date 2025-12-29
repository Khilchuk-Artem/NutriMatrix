import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewPublicRecipesComponent } from './view-public-recipes.component';

describe('ViewPublicRecipesComponent', () => {
  let component: ViewPublicRecipesComponent;
  let fixture: ComponentFixture<ViewPublicRecipesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewPublicRecipesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewPublicRecipesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
