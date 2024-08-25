import { Component } from '@angular/core';
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
    MatRipple
  ],
  templateUrl: './my-secrets.component.html',
  styleUrl: './my-secrets.component.css',
  host: {
    class: 'w-100'
  }
})
export class MySecretsComponent {
  public tableHeaders = ['sentBy', 'received', 'expires', 'openIcon'];

  public secrets = [
    {
      sentBy: 'Alice@example.com',
      received: '2024-08-24T16:30:00Z'
    },
    {
      sentBy: 'Bob@example.com',
      received: '2024-08-24T16:30:00Z'
    }
  ];
}
