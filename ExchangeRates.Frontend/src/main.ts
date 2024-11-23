import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { LOCALE_ID, PLATFORM_ID } from '@angular/core';
import { TranslationService } from './app/translate/translation.service';
import { provideHttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { LanguageService } from './app/translate/language.service';

let language = 'en';
if (isPlatformBrowser(PLATFORM_ID)) {
  language = localStorage.getItem('language') || language;
}

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    { provide: LOCALE_ID, useValue: language },
    LanguageService,
    TranslationService,
    { provide: PLATFORM_ID, useValue: 'browser' }
  ]
}).catch((err) => console.error(err));