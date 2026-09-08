import { Component, OnInit } from '@angular/core';
import {
  Expense,
  ExpenseService,
  ExpenseSummary,
  CategorySpending,
  MonthlySpending,
  Category,
  ExpenseFilter,
} from '../../services/expense.service';
import { ExpenseListComponent } from '../../components/expense-list/expense-list.component';
import { ExpenseFormComponent } from '../../components/expense-form/expense-form.component';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-dashboard',
  imports: [
    ExpenseListComponent,
    ExpenseFormComponent,
    DecimalPipe,
    FormsModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent implements OnInit {
  expenses: Expense[] = [];
  expenseToEdit: Expense | null = null;
  summary: ExpenseSummary | null = null;
  categorySpending: CategorySpending[] = [];
  monthlySpending: MonthlySpending[] = [];

  categories: Category[] = [];
  selectedCategoryId: number | null = null;
  startDate: string = '';
  endDate: string = '';
  minAmount: number | null = null;
  maxAmount: number | null = null;

  maxDate = new Date().toLocaleDateString('en-CA');
  filtersApplied: boolean = false;

  constructor(
    private expenseService: ExpenseService,
    private authService: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loadExpenses();
    this.loadSummary();
    this.loadCategorySpending();
    this.loadMonthlySpending();
    this.loadCategories();
  }

  loadExpenses(): void {
    const filter: ExpenseFilter = {};
    if (this.selectedCategoryId !== null)
      filter.categoryId = this.selectedCategoryId;
    if (this.startDate) filter.startDate = this.startDate;
    if (this.endDate) filter.endDate = this.endDate;
    if (this.minAmount !== null) filter.minAmount = this.minAmount;
    if (this.maxAmount !== null) filter.maxAmount = this.maxAmount;

    this.expenseService.getExpenses(filter).subscribe({
      next: (expenses) => {
        this.expenses = expenses;
      },
      error: (error) => {
        console.error('Failed to load expenses:', error);
      },
    });
  }

  loadSummary(): void {
    this.expenseService.getSummary().subscribe({
      next: (summary) => {
        this.summary = summary;
      },
      error: (error) => {
        console.error('Failed to load summary:', error);
      },
    });
  }

  loadCategorySpending(): void {
    this.expenseService.getSpendingByCategory().subscribe({
      next: (categorySpending) => {
        this.categorySpending = categorySpending;
      },
      error: (error) => {
        console.error('Failed to load category spending:', error);
      },
    });
  }

  loadMonthlySpending(): void {
    this.expenseService.getMonthlySpending().subscribe({
      next: (monthlySpending) => {
        this.monthlySpending = monthlySpending;
      },
      error: (error) => {
        console.error('Failed to load monthly spending:', error);
      },
    });
  }

  loadCategories(): void {
    this.expenseService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Failed to load categories:', error);
      },
    });
  }

  onExpenseCreated(): void {
    this.loadExpenses();
    this.loadSummary();
    this.loadCategorySpending();
    this.loadMonthlySpending();
  }

  onExpenseEdit(expense: Expense): void {
    this.expenseToEdit = expense;
  }

  onExpenseUpdated(): void {
    this.expenseToEdit = null;
    this.loadExpenses();
    this.loadSummary();
    this.loadCategorySpending();
    this.loadMonthlySpending();
  }

  onEditCanceled(): void {
    this.expenseToEdit = null;
  }

  onExpenseDeleted(deletedExpenseId: number): void {
    if (this.expenseToEdit?.id == deletedExpenseId) this.expenseToEdit = null;
    this.loadExpenses();
    this.loadSummary();
    this.loadCategorySpending();
    this.loadMonthlySpending();
  }

  applyFilters(): void {
    this.filtersApplied = true;
    this.loadExpenses();
  }

  clearFilters(): void {
    this.selectedCategoryId = null;
    this.startDate = '';
    this.endDate = '';
    this.minAmount = null;
    this.maxAmount = null;
    this.filtersApplied = false;
    this.loadExpenses();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
