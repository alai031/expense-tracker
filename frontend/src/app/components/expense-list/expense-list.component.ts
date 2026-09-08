import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Expense, ExpenseService } from '../../services/expense.service';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-expense-list',
  imports: [DecimalPipe],
  templateUrl: './expense-list.component.html',
  styleUrl: './expense-list.component.css',
})
export class ExpenseListComponent {
  @Input() expenses: Expense[] = [];
  @Input() filtersApplied = false;
  @Output() expenseDeleted = new EventEmitter<number>();
  @Output() expenseEdit = new EventEmitter<Expense>();

  constructor(private expenseService: ExpenseService) {}

  editExpense(expense: Expense): void {
    this.expenseEdit.emit(expense);
  }

  deleteExpense(id: number): void {
    if (!confirm('Are you sure you want to delete this expense?')) return;
    this.expenseService.deleteExpense(id).subscribe({
      next: () => {
        this.expenseDeleted.emit(id);
      },
      error: (error) => {
        console.error('Failed to delete expense:', error);
      },
    });
  }
}
