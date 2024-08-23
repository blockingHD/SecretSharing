import {Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {AsyncPipe, JsonPipe} from "@angular/common";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AsyncPipe, JsonPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'SecretSharing';
  todos: Observable<any> | null = null;
  constructor(private httpClient: HttpClient) {
  }

  ngOnInit() {
    this.todos = this.httpClient.get('/api/todos')
  }
}
