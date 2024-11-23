import { Component, OnInit } from '@angular/core';
import { ExchangeRatesService } from '../../services/exchange-rates.service';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RoleService } from '../../auth/role.service';
import { AuthService } from '../../auth/auth.service';
import { TranslatePipe } from '../../translate/translate.pipe';

@Component({
  selector: 'app-exchange-rates',
  standalone: true,
  templateUrl: './exchange-rates.component.html',
  styleUrls: ['./exchange-rates.component.css'],
  providers: [DatePipe],
  imports: [FormsModule, CommonModule, TranslatePipe]
})
export class ExchangeRatesComponent implements OnInit {
  currencyCode: string = '';
  date: string = '';
  toCurrency: string = '';
  amount: number = 0;
  convertedAmount: number | null = null;
  exchangeRates: any[] = [];
  searchResults: any[] = [];
  userRole: string | null = null;

  constructor(private exchangeRatesService: ExchangeRatesService, private authorizationService: AuthService, private router: Router, private roleService: RoleService) {}

  ngOnInit() {
    this.userRole = this.roleService.getRole();
    this.exchangeRatesService.getAllExchangeRates().subscribe(rates => {
      this.exchangeRates = rates.data;
    });
  }

  logout() {
    this.authorizationService.logout();
    this.router.navigate(['/login']);
  }

  search() {
    this.exchangeRatesService.searchExchangeRates(this.currencyCode, this.date).subscribe(data => {
      this.searchResults = data;
    });
  }

  convert() {
    if (this.userRole !== 'Admin') {
      alert('You do not have permission to convert rates.');
      return;
    }
    
    if (this.amount > 0 && this.toCurrency) {
      this.exchangeRatesService.convertCurrency(this.toCurrency, this.amount).subscribe(result => {
        this.convertedAmount = result;
      });
    }
  }
}
