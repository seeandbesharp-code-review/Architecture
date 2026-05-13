import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CartModel, AddCartItem, UpdateCartItem, MergeCartItem } from '../Models/Cart';

@Injectable({
  providedIn: 'root'
})

//מקביל לCONTROLLER בAPI
export class CartService {

  private apiUrl = 'http://localhost:5132/api/cart';

  constructor(private http: HttpClient) { }

  // GET: api/cart/{userId}
  GetCartByUserId(userId: number): Observable<CartModel> {
    return this.http.get<CartModel>(`${this.apiUrl}/${userId}`);
  }
  // GET: api/cart/paid/{userId}
GetPaidCartByUserId(userId: number): Observable<CartModel> {
  return this.http.get<CartModel>(`${this.apiUrl}/paid/${userId}`);
}


  // GET: api/cart/unpaid/{userId}
  GetUnpaidCartByUserId(userId: number): Observable<CartModel> {
    return this.http.get<CartModel>(`${this.apiUrl}/unpaid/${userId}`);
  }

  // POST: api/cart/addGift
  AddGiftToCart(dto: AddCartItem): Observable<CartModel> {
    return this.http.post<CartModel>(`${this.apiUrl}/addGift`, dto);
  }
  // POST: api/cart/plusOne
  PlusOne(dto: AddCartItem): Observable<CartModel> {
    return this.http.post<CartModel>(`${this.apiUrl}/plusOne`, dto);
  }

  // POST: api/cart/minusOne
  MinusOne(dto: AddCartItem): Observable<CartModel> {
    return this.http.post<CartModel>(`${this.apiUrl}/minusOne`, dto);
  }


  // PUT: api/cart/updateGift
  UpdateGiftInCart(dto: UpdateCartItem): Observable<CartModel> {
    return this.http.put<CartModel>(`${this.apiUrl}/updateGift`, dto);
  }

  // DELETE: api/cart/deleteGift
  DeleteGiftFromCart(userId: number, giftId: number): Observable<CartModel> {
    return this.http.delete<CartModel>(
      `${this.apiUrl}/deleteGift?userId=${userId}&giftId=${giftId}`
    );
  }

  // PUT: api/cart/purchase
 PurchaseCart(userId: number): Observable<CartModel> {
  return this.http.put<CartModel>(
    `${this.apiUrl}/purchase?userId=${userId}`,
    null
  );
  
}



  // POST: api/cart/mergeCart
  MergeCart(userId: number, items: MergeCartItem[]): Observable<CartModel> {
    return this.http.post<CartModel>(
      `${this.apiUrl}/mergeCart`,
      { userId, guestItems: items }
    );
  }
}
