import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '../../translate/translate.pipe';
import { AuthService } from '../../auth/auth.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css'],
    standalone: true,
    imports: [FormsModule, CommonModule, TranslatePipe]
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  errorMessage: string | null = null;

  constructor(private authService: AuthService, private router: Router) { }

  login(form: NgForm) {
    if (form.invalid) {
      this.errorMessage = 'Both fields are required';
      setTimeout(() => this.errorMessage = null, 5000);
      return;
    }

    const observer = {
      next: (response: any) => {
        localStorage.setItem('token', response.token);
        this.router.navigate(['/']);
      },
      error: (error: any) => {
        this.errorMessage = 'Login failed, wrong username or password';
        setTimeout(() => this.errorMessage = null, 5000);
        console.error('Login failed', error);
      }
    };

    this.authService.login(this.username, this.password).subscribe(observer);
  }
}