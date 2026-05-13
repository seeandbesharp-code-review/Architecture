// ...existing code...
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {  Observable } from 'rxjs';
import { WorkerDto, WorkerModelDto } from '../Models/WorkerModel';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class WorkerService {
  private apiUrl = 'http://localhost:5262/api/workers';

  constructor(private http: HttpClient) {}

getAllWorkers(): Observable<WorkerDto[]> {
  return this.http.get<any[]>(this.apiUrl).pipe(
    map(workers => workers.map(w => ({
    //   workerId: w.workerId,
      firstName: w.firstName,
      lastName: w.lastName,
      dateOfStart: WorkerService.fixDateString(w.dateOfStrat || w.dateOfStart),
    })))
  );
}

// מתודה סטטית שמתקנת פורמט תאריך ל־ISO קריא
private static fixDateString(dateStr: any): string {
  if (!dateStr) return '';
  // מחליף רווח ב־T ומסיר מיקרו-שניות
  return String(dateStr).replace(' ', 'T').split('.')[0];
}



  addWorker(worker: WorkerModelDto): Observable<any> {
    const payload = {
      ...worker,
      dateOfStrat:
        worker.dateOfStart instanceof Date
          ? worker.dateOfStart.toISOString()
          : worker.dateOfStart,
      birthday:
        worker.birthday instanceof Date
          ? worker.birthday.toISOString()
          : worker.birthday
    };
    return this.http.post(this.apiUrl, payload);
  }
}