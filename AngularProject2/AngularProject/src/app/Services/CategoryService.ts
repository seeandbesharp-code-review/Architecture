import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddCategory, Category } from '../Models/Category';


@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private apiUrl = 'http://localhost:5132/api/Category'; // כתובת ה-API שלך

  constructor(private http: HttpClient) { }

  // Get all categories
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  // Get category by id
  getCategoryById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/${id}`);
  }

  // Add new category
  addCategory(category: AddCategory): Observable<Category[]> {
    return this.http.post<Category[]>(this.apiUrl, category);
  }

  // Update category
  updateCategory(id: number, category: Category): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/${id}`, category);
  }

  // Delete category
  deleteCategory(id: number): Observable<Category[]> {
    return this.http.delete<Category[]>(`${this.apiUrl}/${id}`);
  }
}
