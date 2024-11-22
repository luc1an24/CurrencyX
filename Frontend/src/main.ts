import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { isPlatformBrowser } from '@angular/common';
import { LOCALE_ID, PLATFORM_ID } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HTTP_INTERCEPTORS, provideHttpClient } from '@angular/common/http';
import { routes } from './app/app.routes';
import { LanguageService } from './app/services/language.service';
import { TranslationService } from './app/services/translation.service';
import { AuthInterceptor } from './app/authentication/auth.interceptor';

let language = 'en';
if (isPlatformBrowser(PLATFORM_ID)) {
  language = localStorage.getItem('locale') || 'en';
}

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    { provide: LOCALE_ID, useValue: language },
    LanguageService,
    TranslationService,
    { provide: PLATFORM_ID, useValue: 'browser' }
  ]
}).catch((err) => console.error(err));