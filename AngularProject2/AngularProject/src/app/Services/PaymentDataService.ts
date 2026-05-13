import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CartModel } from '../Models/Cart';

@Injectable({ providedIn: 'root' })
export class PaymentDataService {
  private cartData = new BehaviorSubject<CartModel | null>(null);
  cartData$ = this.cartData.asObservable();

  setCartData(data: CartModel) {
    this.cartData.next(data);
  }
}
