import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {CryptoService} from "./crypto.service";
import {AsyncPipe} from "@angular/common";
import {HeaderComponent} from "./header/header.component";
import {PageSelectorComponent} from "./page-selector/page-selector.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AsyncPipe, HeaderComponent, PageSelectorComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'SecretSharing';
  encrypted: string = '';
  constructor(private cryptoService: CryptoService) {
  }

  encryptString(): void {
    this.cryptoService.encrypt("test1234").then((x: Uint8Array) =>
      this.cryptoService.decrypt(x)
        .then(y => this.encrypted = y));
  }
}
