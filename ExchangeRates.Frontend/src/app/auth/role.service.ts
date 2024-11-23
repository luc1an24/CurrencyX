import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private role: string | null = null;

  setRole(role: string): void {
    this.role = role;
  }

  getRole(): string | null {
    if (this.role == null) {
      const token = localStorage.getItem('token');;
      if (token == null)
        return null;

      const decoded: any = jwtDecode(token);
      const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      this.setRole(role);
    }

    return this.role;
  }
}
