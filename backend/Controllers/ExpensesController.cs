using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseTrackerDbContext _context;
    public ExpensesController(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    //GET: api/expenses
    [HttpGet]
    public async Task<IActionResult> GetExpenses()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var expenses = await _context.Expenses
                            .Where(e => e.UserId == userId)
                            .Include(e => e.Category)
                            .OrderByDescending(e => e.ExpenseDate)
                            .Select(e => new
                            {
                                e.Id,
                                e.Description,
                                e.Amount,
                                e.ExpenseDate,
                                e.CategoryId,
                                CategoryName = e.Category.Name,
                                e.CreatedAt
                            })
                            .ToListAsync();
        return Ok(expenses);
    }

    //POST: api/expenses
    [HttpPost]
    public async Task<IActionResult> CreateExpense(Expense expense)
    {
        if (expense.Amount <= 0)
            return BadRequest("Amount must be greater than 0.");

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == expense.CategoryId);
        if (categoryExists == false)
            return BadRequest("Invalid category.");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();
        var userId = int.Parse(userIdClaim.Value);

        expense.Id = 0;
        expense.CreatedAt = DateTime.UtcNow;
        expense.UserId = userId;
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        // return CreatedAtAction(nameof(GetExpenses), new { id = expense.Id }, expense);
        return Ok(expense);
    }

    //PUT: api/expenses/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpense(int id, Expense expense)
    {
        if (expense.Amount <= 0)
            return BadRequest("Amount must be greater than 0.");
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var existingExpense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (existingExpense == null)
            return NotFound("Expense not found.");
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == expense.CategoryId);
        if (categoryExists == false)
            return BadRequest("Invalid category.");
        existingExpense.Amount = expense.Amount;
        existingExpense.Description = expense.Description;
        existingExpense.ExpenseDate = expense.ExpenseDate;
        existingExpense.CategoryId = expense.CategoryId;
        await _context.SaveChangesAsync();
        return Ok(existingExpense);
    }

    //DELETE: api/expenses/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (expense == null)
            return NotFound("Expense not found.");
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}