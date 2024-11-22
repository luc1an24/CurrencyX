import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExchangeRateSearchComponent } from './exchange-rate-search.component';

describe('ExchangeRateSearchComponent', () => {
  let component: ExchangeRateSearchComponent;
  let fixture: ComponentFixture<ExchangeRateSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExchangeRateSearchComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExchangeRateSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
