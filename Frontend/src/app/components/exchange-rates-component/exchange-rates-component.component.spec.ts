import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExchangeRatesComponentComponent } from './exchange-rates-component.component';

describe('ExchangeRatesComponentComponent', () => {
  let component: ExchangeRatesComponentComponent;
  let fixture: ComponentFixture<ExchangeRatesComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExchangeRatesComponentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExchangeRatesComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
