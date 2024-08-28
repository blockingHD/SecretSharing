import {Component} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {MatFormField, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {CryptoService} from "../crypto.service";

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [
    MatFormField,
    MatInput,
    MatLabel
  ],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.css',
  host: {
    class: 'w-100'
  }
})
export class RegistrationComponent {
  constructor(private cryptoService: CryptoService, private httpClient: HttpClient) {
  }

  handleSubmission(password: string): void {
    this.cryptoService.encrypt(password)
      .then(x => console.log(x));
  }
}
