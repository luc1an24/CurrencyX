import { Injectable } from '@angular/core';
import { TranslationService } from './translation.service';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  constructor(private translationService: TranslationService) {}

  setLanguage(language: string): void {
    localStorage.setItem('locale', language);
    this.translationService.loadTranslations(language);
  }

  getLanguage(): string {
    return localStorage.getItem('locale') || 'en';
  }
}
