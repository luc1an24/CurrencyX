import { Component, OnInit } from '@angular/core';
import { TranslationService } from './services/translation.service';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from "./translations/translation.pipe";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
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
