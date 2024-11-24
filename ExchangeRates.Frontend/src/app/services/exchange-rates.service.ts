import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { backendUrls } from '../environment/backendUrls';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class ExchangeRatesService {
  private apiUrl = backendUrls.exchangeRates;

  constructor(private http: HttpClient, private authService: AuthService) { }

  getAllExchangeRates(page: number = 1, pageSize: number = 100): Observable<any> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    
    const headers = this.getAuthHeaders();

    return this.http.get(`${this.apiUrl}`, { params, headers });
  }

  searchExchangeRates(currencyCode: string, date?: string): Observable<any> {
    let params = new HttpParams().set('currencyCode', currencyCode);
    if (date) {
      params = params.set('date', date);
    }

    const headers = this.getAuthHeaders();

    return this.http.get(`${this.apiUrl}/search`, { params, headers });
  }

  convertCurrency(currencyCode: string, value: number): Observable<any> {
    let params = new HttpParams().set('currencyCode', currencyCode).set('value', value.toString());

    const headers = this.getAuthHeaders();

    return this.http.get(`${this.apiUrl}/calculate`, { params, headers });
  }

  private getAuthHeaders(): HttpHeaders {
    const headerToken = this.authService.getHeaderAuthentication();

    if (!headerToken) {
      console.warn('No authentication token found!');
    }

    return new HttpHeaders({
      Authorization: headerToken ?? []
    });
  }
}
