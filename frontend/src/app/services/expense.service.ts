import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
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

export interface ExpenseSummary {
  totalSpent: number;
  expenseCount: number;
  averageExpense: number;
}

export interface Category {
  id: number;
  name: string;
}

export interface CategorySpending {
  category: string;
  total: number;
}

export interface MonthlySpending {
  month: string;
  total: number;
}

export interface ExpenseFilter {
  categoryId?: number;
  startDate?: string;
  endDate?: string;
  minAmount?: number;
  maxAmount?: number;
}

@Injectable({
  providedIn: 'root',
})
export class ExpenseService {
  private apiUrl = `${environment.apiUrl}/expenses`;

  constructor(private http: HttpClient) {}

  getExpenses(filter?: ExpenseFilter): Observable<Expense[]> {
    let params = new HttpParams();
    if (filter?.categoryId !== undefined)
      params = params.set('categoryId', filter.categoryId);
    if (filter?.startDate) params = params.set('startDate', filter.startDate);
    if (filter?.endDate) params = params.set('endDate', filter.endDate);
    if (filter?.minAmount !== undefined)
      params = params.set('minAmount', filter.minAmount);
    if (filter?.maxAmount !== undefined)
      params = params.set('maxAmount', filter.maxAmount);

    return this.http.get<Expense[]>(this.apiUrl, { params });
  }

  getSummary(): Observable<ExpenseSummary> {
    return this.http.get<ExpenseSummary>(`${this.apiUrl}/summary`);
  }

  getSpendingByCategory(): Observable<CategorySpending[]> {
    return this.http.get<CategorySpending[]>(`${this.apiUrl}/by-category`);
  }

  getMonthlySpending(): Observable<MonthlySpending[]> {
    return this.http.get<MonthlySpending[]>(`${this.apiUrl}/monthly`);
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
