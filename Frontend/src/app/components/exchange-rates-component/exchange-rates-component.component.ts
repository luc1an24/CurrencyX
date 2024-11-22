import { Component } from '@angular/core';
import { ExchangeRatesService } from '../../services/exchange-rates.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '../../translations/translation.pipe';

@Component({
  selector: 'app-exchange-rates',
  templateUrl: './exchange-rates.component.html',
  styleUrls: ['./exchange-rates.component.css'],
  imports: [FormsModule, CommonModule, TranslatePipe]
})
export class ExchangeRatesComponent {
  currencyCode: string = '';
  date: string = '';
  fromCurrency: string = '';
  amount: number = 0;
  convertedAmount: number | null = null;
  exchangeRates: any[] = [];
  searchResults: any[] = [];

  constructor(private exchangeRatesService: ExchangeRatesService) {}

  search() {
    this.exchangeRatesService.searchExchangeRates(this.currencyCode, this.date).subscribe(data => {
      this.searchResults = data;
    });
  }

  ngOnInit() {
    this.exchangeRatesService.getAllExchangeRates().subscribe(rates => {
      this.exchangeRates = rates;
    });
  }

  convert() {
    if (this.amount > 0 && this.fromCurrency) {
      this.exchangeRatesService.convertCurrency(this.fromCurrency, this.amount).subscribe(result => {
        this.convertedAmount = result;
      });
    }
  }
}
