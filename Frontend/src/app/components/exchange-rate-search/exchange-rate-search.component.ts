import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExchangeRatesService } from '../../services/exchange-rates.service';

@Component({
  selector: 'app-exchange-rate-search',
  templateUrl: './exchange-rate-search.component.html',
  styleUrls: ['./exchange-rate-search.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class ExchangeRateSearchComponent {
  currencyCode: string = '';
  date: string = '';
  searchResults: any[] = [];

  constructor(private exchangeRatesService: ExchangeRatesService) {}

  search() {
    this.exchangeRatesService.searchExchangeRates(this.currencyCode, this.date).subscribe(data => {
      this.searchResults = data;
    });
  }
}
