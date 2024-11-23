import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';
import { backendUrls } from '../services/backendUrls';
import { RoleService } from './role.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = backendUrls.authentication;

  constructor(private http: HttpClient, private router: Router, private roleService: RoleService) { }

  login(username: string, password: string): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.apiUrl}/login`, { username, password }, { headers }).pipe(
      tap((response: any) => {
        const token = response.token;
        localStorage.setItem('token', token);
        const decoded: any = jwtDecode(token);
        const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        this.roleService.setRole(role);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.roleService.setRole(null);
    this.router.navigate(['/login']);
  }
  
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    return !!token;
  }

  getHeaderAuthentication(): string | null {
    const token = this.getToken();
    if (token) {
      return `Bearer ${token}`;
    }

    return null;
  }

  getRole(): string | null {
    return this.roleService.getRole();
  }
}