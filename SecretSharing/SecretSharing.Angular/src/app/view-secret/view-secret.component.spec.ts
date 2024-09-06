import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewSecretComponent } from './view-secret.component';

describe('ViewSecretComponent', () => {
  let component: ViewSecretComponent;
  let fixture: ComponentFixture<ViewSecretComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewSecretComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewSecretComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
