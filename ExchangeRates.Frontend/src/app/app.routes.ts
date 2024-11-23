import { Routes } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';
import { LoginComponent } from './components/login/login.component';
import { ExchangeRatesComponent } from './components/exchange-rates-component/exchange-rates.component';

export const routes: Routes = [
  { path: '', component: ExchangeRatesComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
];
