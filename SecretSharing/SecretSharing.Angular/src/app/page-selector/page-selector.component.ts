import {Component, EventEmitter, Output, output} from '@angular/core';
import {MatTab, MatTabGroup} from "@angular/material/tabs";
import {MatButtonToggle, MatButtonToggleGroup} from "@angular/material/button-toggle";
import {MatRipple} from "@angular/material/core";
import {Router, RouterLink, RouterLinkActive} from "@angular/router";
import {NgClass} from "@angular/common";

@Component({
  selector: 'app-page-selector',
  standalone: true,
  imports: [
    MatTab,
    MatTabGroup,
    MatButtonToggleGroup,
    MatButtonToggle,
    MatRipple,
    NgClass,
    RouterLinkActive,
    RouterLink
  ],
  templateUrl: './page-selector.component.html',
  styleUrl: './page-selector.component.css'
})
export class PageSelectorComponent {
}
