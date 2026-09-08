import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Expense, ExpenseService } from '../../services/expense.service';

@Component({
  selector: 'app-expense-form',
  imports: [ReactiveFormsModule],
  templateUrl: './expense-form.component.html',
  styleUrl: './expense-form.component.css',
})
export class ExpenseFormComponent implements OnInit, OnChanges {
  @Input() expenseToEdit: Expense | null = null;
  @Output() expenseCreated = new EventEmitter<void>();
  @Output() editCanceled = new EventEmitter<void>();
  @Output() expenseUpdated = new EventEmitter<void>();

  expenseForm: FormGroup;
  categories: any[] = [];
  maxDate = new Date().toLocaleDateString('en-CA');

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

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['expenseToEdit']) {
      if (this.expenseToEdit) {
        this.expenseForm.patchValue({
          amount: this.expenseToEdit.amount,
          description: this.expenseToEdit.description,
          categoryId: this.expenseToEdit.categoryId,
          expenseDate: this.expenseToEdit.expenseDate.split('T')[0],
        });
      } else {
        this.expenseForm.reset();
      }
    }
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

    const expense = this.expenseForm.value;
    if (this.expenseToEdit) {
      this.expenseService
        .updateExpense(this.expenseToEdit.id, expense)
        .subscribe({
          next: () => {
            this.expenseForm.reset();
            this.expenseUpdated.emit();
          },
          error: (error) => {
            console.error('Failed to update expense:', error);
          },
        });
    } else {
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

  cancelEdit(): void {
    this.expenseForm.reset();
    this.editCanceled.emit();
  }
}
