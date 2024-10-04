import {Component} from '@angular/core';
import {MatFormField, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {HttpClient} from "@angular/common/http";
import {CryptoService} from "../crypto.service";

@Component({
  selector: 'app-send-secret',
  standalone: true,
  imports: [
    MatFormField,
    MatLabel,
    MatInput,
    ReactiveFormsModule
  ],
  templateUrl: './send-secret.component.html',
  styleUrl: './send-secret.component.css',
  host: {
    class: 'w-100'
  }
})
export class SendSecretComponent {
  public secretForm: FormGroup;

  constructor(private readonly fb: FormBuilder, private readonly httpClient: HttpClient, private readonly cryptoService: CryptoService) {
    this.secretForm = this.fb.group({
      to: ['', Validators.required],
      secret: ['', Validators.required]
    });
  }

  public sendSecret() {
    this.httpClient.get(`/api/users/${this.secretForm.value.to}/keys`)
      .subscribe(async (resp: any) => {
        const secret = await this.cryptoService.encryptWithPublicKey(resp.publicKey as string, this.secretForm.value.secret);
        const body = JSON.stringify(secret);
        this.httpClient.post(`/api/secrets/${this.secretForm.value.to}`,
          body,
          {
            headers: {
              'Content-Type': 'application/json'
            }
          })
          .subscribe(() => {
            this.secretForm.reset()
          });
      });
  }
}
