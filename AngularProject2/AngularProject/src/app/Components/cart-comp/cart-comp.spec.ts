import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CartComp } from './cart-comp';

describe('CartComp', () => {
  let component: CartComp;
  let fixture: ComponentFixture<CartComp>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartComp]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CartComp);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
