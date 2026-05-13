import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentProcessComponent } from './payment-process-component';

describe('PaymentProcessComponent', () => {
  let component: PaymentProcessComponent;
  let fixture: ComponentFixture<PaymentProcessComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentProcessComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentProcessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
