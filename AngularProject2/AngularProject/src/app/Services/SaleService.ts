// src/app/services/sales.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GiftResume, GiftsWithBuyers } from '../Models/Sales';

@Injectable({
  providedIn: 'root'
})
export class SalesService {
  private apiUrl = 'http://localhost:5132/api/sales'; 

  constructor(private http: HttpClient) { }

  // GET api/sales/gift-resumes
  // מחזיר סיכום מכירות לכל מתנה: כמה כרטיסים נמכרו וכמה הכנסות היו מכל מתנה
  getGiftResumes(): Observable<GiftResume[]> {
    return this.http.get<GiftResume[]>(`${this.apiUrl}/gift-resumes`);
  }

  // GET api/sales/gift-buyers
  // מחזיר רשימת מתנות עם רשימת הרוכשים של כל מתנה
  getGiftWithBuyers(): Observable<GiftsWithBuyers[]> {
    return this.http.get<GiftsWithBuyers[]>(`${this.apiUrl}/gift-buyers`);
  }

  // GET api/sales/gifts-by-price
  // מחזיר רשימת מתנות עם רשימת הרוכשים של כל מתנה, ממוינת לפי מחיר כרטיס מהגבוה לנמוך
  getGiftsByPriceDesc(): Observable<GiftsWithBuyers[]> {
    return this.http.get<GiftsWithBuyers[]>(`${this.apiUrl}/gifts-by-price`);
  }

  // GET api/sales/gifts-by-quantity
  // מחזיר רשימת מתנות עם רשימת הרוכשים של כל מתנה, ממוינת לפי כמות הרוכשים מהגבוה לנמוך
  getGiftsByQuantityDesc(): Observable<GiftsWithBuyers[]> {
    return this.http.get<GiftsWithBuyers[]>(`${this.apiUrl}/gifts-by-quantity`);
  }
}
