import { Component, OnInit } from '@angular/core';
import { Expense, ExpenseService } from '../../services/expense.service';
import { ExpenseListComponent } from '../../components/expense-list/expense-list.component';
import { ExpenseFormComponent } from '../../components/expense-form/expense-form.component';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  imports: [ExpenseListComponent, ExpenseFormComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent implements OnInit {
  expenses: Expense[] = [];
  constructor(
    private expenseService: ExpenseService,
    private authService: AuthService,
    private router: Router,
  ) {}
  ngOnInit(): void {
    this.loadExpenses();
  }

  loadExpenses(): void {
    this.expenseService.getExpenses().subscribe({
      next: (expenses) => {
        this.expenses = expenses;
      },
      error: (error) => {
        console.error('Failed to load expenses:', error);
      },
    });
  }

  onExpenseCreated(): void {
    this.loadExpenses();
  }

  onExpenseDeleted(): void {
    this.loadExpenses();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
