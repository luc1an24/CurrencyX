import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { backendUrls } from './backendUrls';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class ExchangeRatesService {
  private apiUrl = backendUrls.exchangeRates;

  constructor(private http: HttpClient, private authService: AuthService) { }

  getAllExchangeRates(page: number = 1, pageSize: number = 100): Observable<any> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    
    const headerToken = this.authService.getHeaderAuthentication();
    const headers = new HttpHeaders({
      Authorization: headerToken ?? []
    });

    return this.http.get(`${this.apiUrl}`, { params, headers });
  }

  searchExchangeRates(currencyCode: string, date?: string): Observable<any> {
    let params = new HttpParams().set('currencyCode', currencyCode);
    if (date) {
      params = params.set('date', date);
    }

    const headerToken = this.authService.getHeaderAuthentication();
    const headers = new HttpHeaders({
      Authorization: headerToken ?? []
    });

    return this.http.get(`${this.apiUrl}/search`, { params, headers });
  }

  convertCurrency(currencyCode: string, value: number): Observable<any> {
    let params = new HttpParams().set('currencyCode', currencyCode).set('value', value.toString());

    const headerToken = this.authService.getHeaderAuthentication();
    const headers = new HttpHeaders({
      Authorization: headerToken ?? []
    });

    return this.http.get(`${this.apiUrl}/calculate`, { params, headers });
  }
}
