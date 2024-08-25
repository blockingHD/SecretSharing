import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SendSecretComponent } from './send-secret.component';

describe('SendSecretComponent', () => {
  let component: SendSecretComponent;
  let fixture: ComponentFixture<SendSecretComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SendSecretComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SendSecretComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
