import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private role: string | null = null;

  setRole(role: string | null): void {
    this.role = role;
  }

  getRole(): string | null {
    return this.role;
  }
}
