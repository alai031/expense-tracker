using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseTrackerDbContext _context;
    public ExpensesController(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetExpenses()
    {
        var expenses = await _context.Expenses
                            .Include(e => e.Category)
                            .OrderByDescending(e => e.ExpenseDate)
                            .Select(e => new
                            {
                                e.Id,
                                e.Amount,
                                e.ExpenseDate,
                                e.CategoryId,
                                CategoryName = e.Category.Name
                            })
                            .ToListAsync();
        return Ok(expenses);
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense(Expense expense)
    {
        if (expense.Amount <= 0)
            return BadRequest("Amount must be greater than 0.");

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == expense.CategoryId);
        if (categoryExists == false)
            return BadRequest("Invalid category.");
        expense.Id = 0;
        expense.CreatedAt = DateTime.UtcNow;
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetExpenses), new { id = expense.Id }, expense);
    }
}