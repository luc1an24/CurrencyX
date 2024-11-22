import { Injectable } from '@angular/core';
import * as engLanguage from '../translations/english.json';
import * as sloLanguage from '../translations/slovenian.json';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private translations: any = {};

  constructor() { }

  loadTranslations(language: string): void {
    this.translations = {};

    switch (language) {
      case 'en':
        this.translations = engLanguage;
        break;
      case 'sl':
        this.translations = sloLanguage;
        break;
      default:
        this.translations = engLanguage;
        break;
    }

  }

  translate(key: string): string {
    return this.translations[key] || key;
  }
}