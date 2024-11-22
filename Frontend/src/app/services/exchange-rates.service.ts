import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExchangeRatesService {
  private apiUrl = 'https://your-api-domain/api/exchange-rates'; // Update with your API base URL

  constructor(private http: HttpClient) {}

  getAllExchangeRates(page: number = 1, pageSize: number = 100): Observable<any> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get(`${this.apiUrl}`, { params });
  }

  searchExchangeRates(currencyCode: string, date?: string): Observable<any> {
    let params = new HttpParams().set('currencyCode', currencyCode);
    if (date) {
      params = params.set('date', date);
    }
    return this.http.get(`${this.apiUrl}/search`, { params });
  }

  convertCurrency(currencyCode: string, value: number): Observable<any> {
    let params = new HttpParams().set('currencyCode', currencyCode).set('value', value.toString());
    return this.http.get(`${this.apiUrl}/calculate`, { params });
  }
}
