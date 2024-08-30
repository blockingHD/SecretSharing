import {Component, OnInit} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {CryptoService} from "./crypto.service";
import {AsyncPipe} from "@angular/common";
import {HeaderComponent} from "./header/header.component";
import {PageSelectorComponent} from "./page-selector/page-selector.component";
import {HttpClient} from "@angular/common/http";
import {catchError, map, Observable, of, take} from "rxjs";
import {RegistrationComponent} from "./registration/registration.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AsyncPipe, HeaderComponent, PageSelectorComponent, RegistrationComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'SecretSharing';
  encrypted: string = '';
  isUserRegistered: Observable<boolean> | null = null;

  constructor(private cryptoService: CryptoService, private httpClient: HttpClient) {
  }

  ngOnInit(): void {
    this.isUserRegistered = this.httpClient.get('/api/user/keys')
      .pipe(
        map(() => true),
        catchError(() => of(false))
      );
  }
}
