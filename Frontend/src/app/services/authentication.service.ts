import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';
import { BackendUrl } from '../common/backend.url';
import { RoleService } from './role.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private apiUrl = BackendUrl.urlString;

  constructor(private http: HttpClient, private router: Router, private roleService: RoleService) { }

  login(username: string, password: string): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.apiUrl}/Authentication/login`, { username, password }, { headers }).pipe(
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

  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    return !!token;
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getRole(): string | null {
    return this.roleService.getRole();
  }

  hasRole(expectedRole: string): boolean {
    const role = this.getRole();
    return role === expectedRole;
  }
}
