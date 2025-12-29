import { TestBed } from '@angular/core/testing';

import { MealRecordsService } from './meal-records.service';

describe('MealRecordsService', () => {
  let service: MealRecordsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MealRecordsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
