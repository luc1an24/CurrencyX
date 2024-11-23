import { Component, OnInit } from '@angular/core';
import { TranslationService } from './translate/translation.service';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from "./translate/translate.pipe";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [RouterModule, FormsModule, TranslatePipe]
})
export class AppComponent implements OnInit {
  title = 'CurrencyX';

  constructor(private translationService: TranslationService) {}

  ngOnInit(): void {
    const locale = this.getLocale() || 'en';
    this.translationService.loadTranslations(locale);
  }

  setLanguage(locale: string) {
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('locale', locale);
    }
    this.translationService.loadTranslations(locale);
    location.reload();
  }

  private getLocale(): string | null {
    return typeof localStorage !== 'undefined' ? localStorage.getItem('locale') : null;
  }
}
