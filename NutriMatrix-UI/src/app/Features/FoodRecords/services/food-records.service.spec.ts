import { TestBed } from '@angular/core/testing';

import { FoodRecordsService } from './food-records.service';

describe('FoodRecordsService', () => {
  let service: FoodRecordsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FoodRecordsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
