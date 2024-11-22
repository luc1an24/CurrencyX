import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExchangeRatesService } from '../../services/exchange-rates.service';

@Component({
  selector: 'app-currency-converter',
  templateUrl: './currency-converter.component.html',
  styleUrls: ['./currency-converter.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class CurrencyConverterComponent {
  fromCurrency: string = '';
  toCurrency: string = '';
  amount: number = 0;
  convertedAmount: number | null = null;

  constructor(private exchangeRatesService: ExchangeRatesService) {}

  convert() {
    if (this.amount > 0 && this.fromCurrency) {
      this.exchangeRatesService.convertCurrency(this.fromCurrency, this.amount).subscribe(result => {
        this.convertedAmount = result;
      });
    }
  }
}
