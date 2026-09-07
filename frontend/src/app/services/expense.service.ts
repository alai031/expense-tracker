import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Expense {
  id: number;
  amount: number;
  description: string;
  expenseDate: string;
  categoryId: number;
  categoryName: string;
}

export interface Category {
  id: number;
  name: string;
}

@Injectable({
  providedIn: 'root',
})
export class ExpenseService {
  private apiUrl = `${environment.apiUrl}/expenses`;

  constructor(private http: HttpClient) {}

  getExpenses(): Observable<Expense[]> {
    return this.http.get<Expense[]>(this.apiUrl);
  }

  createExpense(expense: Omit<Expense, 'id'>): Observable<Expense> {
    return this.http.post<Expense>(this.apiUrl, expense);
  }

  updateExpense(id: number, expense: Omit<Expense, 'id'>): Observable<Expense> {
    return this.http.put<Expense>(`${this.apiUrl}/${id}`, expense);
  }

  deleteExpense(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${environment.apiUrl}/categories`);
  }
}
