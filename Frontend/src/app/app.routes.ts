import { Routes } from '@angular/router';
import { ExchangeRatesComponent } from './components/exchange-rates-component/exchange-rates-component.component';
import { LoginComponent } from './components/login/login.component';
import { AuthGuard } from './authentication/auth.guard';

export const routes: Routes = [
  { path: '', component: ExchangeRatesComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
];
