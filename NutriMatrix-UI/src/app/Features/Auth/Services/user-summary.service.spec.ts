import { TestBed } from '@angular/core/testing';

import { UserSummaryService } from './user-summary.service';

describe('UserSummaryService', () => {
  let service: UserSummaryService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserSummaryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
