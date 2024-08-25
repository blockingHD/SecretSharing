import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MySecretsComponent } from './my-secrets.component';

describe('MySecretsComponent', () => {
  let component: MySecretsComponent;
  let fixture: ComponentFixture<MySecretsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MySecretsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MySecretsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
