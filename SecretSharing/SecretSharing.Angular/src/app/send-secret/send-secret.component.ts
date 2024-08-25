import { Component } from '@angular/core';
import {MatFormField, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";

@Component({
  selector: 'app-send-secret',
  standalone: true,
  imports: [
    MatFormField,
    MatLabel,
    MatInput
  ],
  templateUrl: './send-secret.component.html',
  styleUrl: './send-secret.component.css',
  host: {
    class: 'w-100'
  }
})
export class SendSecretComponent {

}
