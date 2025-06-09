import { TestBed } from '@angular/core/testing';

import { PendingRecordsService } from './pending-records.service';

describe('PendingRecordsService', () => {
  let service: PendingRecordsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PendingRecordsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
