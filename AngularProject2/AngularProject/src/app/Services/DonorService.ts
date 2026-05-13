import { ChangeDetectorRef, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddDonor, DonorModel, UpdateDonor } from '../Models/Donor';

export interface Donor {
  id: number;
  name: string;
  email: string;
  amount: number;
  date: string;
}

@Injectable({
  providedIn: 'root'
})
export class DonorService {
  private apiUrl = 'http://localhost:5132/api/Donors'; // הכתובת של ה-API שלך

  constructor(private http: HttpClient) { }

  // מביא את כל התורמים
  getAllDonors(): Observable<Donor[]> {
    return this.http.get<Donor[]>(this.apiUrl);
  }

  getDonors(): Observable<DonorModel[]> {
    return this.http.get<DonorModel[]>(this.apiUrl);
  }


  // מביא תורם לפי ID
  getDonorById(id: number): Observable<DonorModel> {
    return this.http.get<DonorModel>(`${this.apiUrl}/${id}`);
  }

  // מוסיף תורם חדש
  createDonor(donor: AddDonor): Observable<Donor> {
    return this.http.post<Donor>(this.apiUrl, donor);
  }

  // מעדכן תורם קיים
updateDonor(donor: UpdateDonor): Observable<Donor> {
  return this.http.put<Donor>(`${this.apiUrl}/${donor.donorId}`, donor);
}

  // מוחק תורם
  // deleteDonor(id: number): Observable<Donor> {
  //   return this.http.delete<Donor>(`${this.apiUrl}/${id}`);
  // }

  deleteDonor(id: number) {
  return this.http.delete(
    `${this.apiUrl}/${id}`,
    { responseType: 'text' }
  );
}


  // מחפש תורמים לפי שם מלא
  searchDonorsByName(name: string): Observable<Donor[]> {
  return this.http.get<Donor[]>(`${this.apiUrl}/searchDonorByFullName`, {
    params: { name }
  });
}

}
 