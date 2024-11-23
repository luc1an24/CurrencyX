import { Injectable } from '@angular/core';
import * as enLocale from './english.json';
import * as slLocale from './slovenian.json';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private translations: any = {};

  constructor() { }

  loadTranslations(locale: string): void {
    this.translations = {};

    switch (locale) {
      case 'en':
        this.translations = enLocale;
        break;
      case 'sl':
        this.translations = slLocale;
        break;
      default:
        this.translations = enLocale;
        break;
    }

  }

  translate(key: string): string {
    return this.translations[key] || key;
  }
}