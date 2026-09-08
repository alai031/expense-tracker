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
    public async Task<IActionResult> GetExpenses([FromQuery] ExpenseFilter filter)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var query = _context.Expenses.Where(e => e.UserId == userId);

        if (filter.CategoryId.HasValue)
            query = query.Where(e => e.CategoryId == filter.CategoryId.Value);
        if (filter.StartDate.HasValue)
            query = query.Where(e => e.ExpenseDate >= filter.StartDate.Value);
        if (filter.EndDate.HasValue)
            query = query.Where(e => e.ExpenseDate <= filter.EndDate.Value);
        if (filter.MinAmount.HasValue)
            query = query.Where(e => e.Amount >= filter.MinAmount.Value);
        if (filter.MaxAmount.HasValue)
            query = query.Where(e => e.Amount <= filter.MaxAmount.Value);

        var expenses = await query
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

    //GET: api/expenses/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var query = _context.Expenses.Where(e => e.UserId == userId);
        var total = await query.SumAsync(e => e.Amount);
        var count = await query.CountAsync();
        var average = count > 0 ? await query.AverageAsync(e => e.Amount) : 0;
        return Ok(new
        {
            totalSpent = total,
            expenseCount = count,
            averageExpense = average
        });
    }

    //GET: api/expenses/by-category
    [HttpGet("by-category")]
    public async Task<IActionResult> GetSpendingByCategory()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var results = await _context.Expenses
                        .Where(e => e.UserId == userId)
                        .GroupBy(e => e.Category.Name)
                        .Select(g => new
                        {
                            category = g.Key,
                            total = g.Sum(e => e.Amount)
                        }).OrderByDescending(x => x.total)
                        .ToListAsync();
        return Ok(results);
    }

    //GET: api/expenses/monthly
    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlySpending()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var results = await _context.Expenses
                        .Where(e => e.UserId == userId)
                        .GroupBy(e => new
                        {
                            Year = e.ExpenseDate.Year,
                            Month = e.ExpenseDate.Month
                        })
                        .Select(g => new
                        {
                            year = g.Key.Year,
                            month = g.Key.Month,
                            total = g.Sum(e => e.Amount)
                        })
                        .OrderBy(x => x.year)
                        .ThenBy(x => x.month)
                        .ToListAsync();
        var response = results.Select(x => new
        {
            month = $"{x.year}-{x.month:D2}",
            total = x.total
        });
        return Ok(response);
    }

    //POST: api/expenses
    [HttpPost]
    public async Task<IActionResult> CreateExpense(Expense expense)
    {
        if (expense.Amount <= 0)
            return BadRequest("Amount must be greater than 0.");

        if (expense.ExpenseDate > DateOnly.FromDateTime(DateTime.UtcNow))
            return BadRequest("Expense date cannot be in the future.");

        if (string.IsNullOrWhiteSpace(expense.Description))
            return BadRequest("Description is required.");

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
        return StatusCode(201, expense);
    }

    //PUT: api/expenses/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpense(int id, Expense expense)
    {
        if (expense.Amount <= 0)
            return BadRequest("Amount must be greater than 0.");
        if (expense.ExpenseDate > DateOnly.FromDateTime(DateTime.UtcNow))
            return BadRequest("Expense date cannot be in the future.");
        if (string.IsNullOrWhiteSpace(expense.Description))
            return BadRequest("Description is required.");
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