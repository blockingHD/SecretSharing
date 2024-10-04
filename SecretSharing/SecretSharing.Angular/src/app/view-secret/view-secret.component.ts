import {Component, Inject} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle
} from '@angular/material/dialog';
import {catchError, forkJoin, Observable, of, switchMap} from "rxjs";
import {Secret} from "../my-secrets/my-secrets.component";
import {AsyncPipe} from "@angular/common";
import {MatButton} from "@angular/material/button";
import {CryptoService, KeyMaterial} from "../crypto.service";
import {MatFormField, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-view-secret',
  standalone: true,
  imports: [
    AsyncPipe,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatLabel,
    MatFormField,
    MatInput,
    MatButton
  ],
  templateUrl: './view-secret.component.html',
  styleUrl: './view-secret.component.css'
})
export class ViewSecretComponent {
  public secret$: Observable<string> | null = null;

  constructor(private readonly httpClient: HttpClient,
              private readonly cryptoService: CryptoService,
              private readonly dialogRef: MatDialogRef<ViewSecretComponent>,
              private readonly snackbar: MatSnackBar,
              @Inject(MAT_DIALOG_DATA) public readonly data: Secret
  ) {
  }

  viewSecret(password: string) {
    this.secret$ = forkJoin([this.httpClient.get('/api/users/me/keys'), this.httpClient.get<SecretValue>(`/api/secrets/${this.data.secretId}`)])
      .pipe(switchMap(async ([keys, secret]) => {
        const privateKey = await this.cryptoService.decrypt(KeyMaterial.fromObj(keys), password);
        return await this.cryptoService.decryptWithPrivateKey(privateKey, secret.secretValue);
      }),
        catchError(() => {
          this.snackbar.open('Failed to decrypt secret');
          return of();
        }));
  }

  closeDialog() {
    this.dialogRef.close();
  }

  copyAndCloseDialog(secretValue: string) {
    navigator.clipboard.writeText(secretValue).then(() => this.dialogRef.close());
  }
}

type SecretValue = Secret & {secretValue: string};
