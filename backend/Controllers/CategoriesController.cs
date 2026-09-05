using ExpenseTracker.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ExpenseTrackerDbContext _context;

    public CategoriesController(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
                            .OrderBy(c => c.Name)
                            .ToListAsync();

        return Ok(categories);
    }
}