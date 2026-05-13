import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddGift, GiftModel, UpdateGift } from '../Models/Gift';

@Injectable({
  providedIn: 'root'
})
export class GiftsService {

  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5132/api/Gifts';

  getGifts() {
    return this.http.get<GiftModel[]>(`${this.baseUrl}`);
  }
  sortByName(name: string): Observable<GiftModel[]> {
    return this.http.get<GiftModel[]>(
      `${this.baseUrl}/sortByName`,
      { params: { name } }
    );
  }
  getGiftById(id: number): Observable<GiftModel> {
    return this.http.get<GiftModel>(`${this.baseUrl}/${id}`);
  }

  sortByBuyers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/sortByBuyers`);
  }

  sortByDonorName(name: string): Observable<GiftModel[]> {
    return this.http.get<GiftModel[]>(
      `${this.baseUrl}/sortByDonorName`,
      { params: { name } }
    );
  }

  orderByPrice(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/OrderByPrice`);
  }

  orderByCategory(id: number): Observable<GiftModel[]> {
    return this.http.get<GiftModel[]>(
      `${this.baseUrl}/OrderByCategory`,
      { params: { id } }
    );
  }


  addGift(gift: AddGift) {
    return this.http.post(`${this.baseUrl}`, gift);
  }

  updateGift(id: number, gift: UpdateGift) {
    return this.http.put(`${this.baseUrl}/${id}`, gift);
  }

  deleteGift(id: number): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/${id}`);
  }

  ruffleGift(giftId: number): Observable<string> {
    return this.http.post<string>(`http://localhost:5132/api/Lottery/${giftId}`, {});
  }

}
