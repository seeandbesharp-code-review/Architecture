import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginDto, RegisterDto } from '../Models/Author';
import { jwtDecode } from 'jwt-decode';

import { BehaviorSubject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthorService {

  private apiUrl = 'http://localhost:5132/api/Auth';
  private roleSubject = new BehaviorSubject<string>(this.getUserRole() || ''); // מאתחל עם התפקיד הנוכחי או מחרוזת ריקה
  role$ = this.roleSubject.asObservable(); // מיועד למנויים


  constructor(private http: HttpClient) { }

  register(dto: RegisterDto): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, dto);
  }

  // login(dto: LoginDto): Observable<any> {
  //             const role = this.getUserRole();
  //         this.roleSubject.next(role || ''); // מעדכן את ה-BehaviorSubject עם התפקיד הנוכחי לאחר ההתחברות

  //   return this.http.post<any>(`${this.apiUrl}/login`, dto);
  // }

  login(dto: LoginDto): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, dto).pipe(
      tap(response => {
        localStorage.setItem('authToken', response.token);

        const role = this.getUserRole();
        this.roleSubject.next(role || '');
      })
    );
  }

  getUserRole(): string | null {
    const token = localStorage.getItem('authToken');
    if (!token) return null;

    try {
      const decoded: any = jwtDecode(token);
      return decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }
  isManager(): boolean {
    return this.getUserRole() === 'manager';
  }

}
