import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle
} from '@angular/material/dialog';
import {forkJoin, map, Observable, switchMap} from "rxjs";
import {Secret} from "../my-secrets/my-secrets.component";
import {AsyncPipe} from "@angular/common";
import {MatButton} from "@angular/material/button";
import {CryptoService, KeyMaterial} from "../crypto.service";

@Component({
  selector: 'app-view-secret',
  standalone: true,
  imports: [
    AsyncPipe,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatButton
  ],
  templateUrl: './view-secret.component.html',
  styleUrl: './view-secret.component.css'
})
export class ViewSecretComponent implements OnInit {
  public secret$: Observable<string> | null = null;

  constructor(private httpClient: HttpClient,
              private cryptoService: CryptoService,
              private dialogRef: MatDialogRef<ViewSecretComponent>,
              @Inject(MAT_DIALOG_DATA) public data: Secret
  ) {
  }

  ngOnInit(): void {
    this.secret$ = forkJoin([this.httpClient.get('/api/users/me/keys'), this.httpClient.get<SecretValue>(`/api/secrets/${this.data.secretId}`)])
      .pipe(switchMap(async ([keys, secret]) => {
        const privateKey = await this.cryptoService.decrypt(KeyMaterial.fromObj(keys), 'Test');
        return await this.cryptoService.decryptWithPrivateKey(privateKey, secret.secretValue);
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
