import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ExpenseService } from '../../services/expense.service';

@Component({
  selector: 'app-expense-form',
  imports: [ReactiveFormsModule],
  templateUrl: './expense-form.component.html',
  styleUrl: './expense-form.component.css',
})
export class ExpenseFormComponent implements OnInit {
  @Output() expenseCreated = new EventEmitter<void>();

  expenseForm: FormGroup;
  categories: any[] = [];

  constructor(
    private fb: FormBuilder,
    private expenseService: ExpenseService,
  ) {
    this.expenseForm = this.fb.group({
      amount: ['', [Validators.required, Validators.min(0.01)]],
      description: ['', Validators.required],
      categoryId: ['', Validators.required],
      expenseDate: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadCategories();
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

  onSubmit(): void {
    if (this.expenseForm.invalid) {
      this.expenseForm.markAllAsTouched();
      return;
    }

    this.expenseService.createExpense(this.expenseForm.value).subscribe({
      next: () => {
        this.expenseForm.reset();
        this.expenseCreated.emit();
      },
      error: (error) => {
        console.error('Failed to create expense:', error);
      },
    });
  }
}
