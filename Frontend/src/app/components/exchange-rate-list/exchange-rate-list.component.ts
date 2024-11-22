import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExchangeRatesService } from '../../services/exchange-rates.service';

@Component({
  selector: 'app-exchange-rate-list',
  templateUrl: './exchange-rate-list.component.html',
  styleUrls: ['./exchange-rate-list.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class ExchangeRateListComponent implements OnInit {
  exchangeRates: any[] = [];
  constructor(private exchangeRatesService: ExchangeRatesService) {}

  ngOnInit() {
    this.loadExchangeRates();
  }

  loadExchangeRates() {
    this.exchangeRatesService.getAllExchangeRates().subscribe(data => {
      this.exchangeRates = data;
    });
  }
}
