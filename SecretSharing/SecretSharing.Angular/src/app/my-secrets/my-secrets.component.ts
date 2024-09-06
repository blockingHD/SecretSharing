import {Component, OnInit} from '@angular/core';
import {MatIcon} from "@angular/material/icon";
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable
} from "@angular/material/table";
import {MatRipple} from "@angular/material/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {AsyncPipe, DatePipe} from "@angular/common";
import {MatDialog} from "@angular/material/dialog";
import {ViewSecretComponent} from "../view-secret/view-secret.component";

@Component({
  selector: 'app-my-secrets',
  standalone: true,
  imports: [
    MatIcon,
    MatTable,
    MatHeaderCell,
    MatCell,
    MatColumnDef,
    MatHeaderCellDef,
    MatCellDef,
    MatHeaderRow,
    MatRow,
    MatRowDef,
    MatHeaderRowDef,
    MatRipple,
    AsyncPipe,
    DatePipe
  ],
  templateUrl: './my-secrets.component.html',
  styleUrl: './my-secrets.component.css',
  host: {
    class: 'w-100'
  }
})
export class MySecretsComponent implements OnInit{
  public tableHeaders = ['sentBy', 'received', 'expires', 'openIcon'];
  public secrets$: Observable<Secret[]> | null = null;

  constructor(private httpClient: HttpClient, private dialog: MatDialog) {
  }

  ngOnInit(): void {
      this.secrets$ =
        this.httpClient.get<Secret[]>('/api/secrets/');
  }

  calculateExpiration(createdDate: string): string {
    const expirationDate = new Date(createdDate);
    expirationDate.setDate(expirationDate.getDate() + 1);
    return expirationDate.toISOString();
  }

  openSecret(secret: Secret) {
    this.dialog.open(ViewSecretComponent, {
      data: secret
    });
  }
}

export interface Secret {
  secretId: number;
  senderEmail: string;
  createdDate: string;
}
