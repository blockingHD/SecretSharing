import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {CryptoService} from "../crypto.service";
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {Router} from "@angular/router";

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule
  ],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.css',
  host: {
    class: 'w-100'
  }
})
export class RegistrationComponent implements OnInit {
  @Output() registered = new EventEmitter<void>();

  public passwordForm: FormGroup<{
    password: FormControl<string | null>
  }> | undefined;

  constructor(private cryptoService: CryptoService,
              private formBuilder: FormBuilder,
              private httpClient: HttpClient) {
  }

  ngOnInit(): void {
    this.passwordForm = this.formBuilder.group({
      password: ['', Validators.required]
    });
  }

  handleSubmission(): void {
    if (!this.passwordForm?.valid) {
      return;
    }

    const password = this.passwordForm.value.password!;

    this.cryptoService.encrypt(password)
      .then(async x => {
        this.httpClient.post('/api/users/me/keys', x.json, {
          headers: {
            'Content-Type': 'application/json'
          }
        }).subscribe(
          () => this.registered!.emit()
        );
      });
  }

}
