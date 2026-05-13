import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TypeOfWork, WorkModel } from '../Models/TestWork'; 
import { BossModel } from '../Models/TestBoss';

@Injectable({
  providedIn: 'root'
})
export class TestService {
private apiUrl = 'http://localhost:5262/api/work';

   constructor(private http: HttpClient) {}

  getAllWork(): Observable<WorkModel[]> {
    return this.http.get<WorkModel[]>(`${this.apiUrl}/GetAllWork`);
  }

  getAllBoss(): Observable<BossModel[]> {
    return this.http.get<BossModel[]>(`${this.apiUrl}/GetAllBoss`);
  }

  createWork(work: WorkModel): Observable<WorkModel> {
    work.typeOfWork = TypeOfWork.Field;
    return this.http.post<WorkModel>(`${this.apiUrl}/createWork`, work);
  }

  createBoss(boss: BossModel): Observable<BossModel> {
    boss.typeOfWork = TypeOfWork.Management;
    return this.http.post<BossModel>(`${this.apiUrl}/createBoss`, boss);
  }

}
