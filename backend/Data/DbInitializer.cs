using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ExpenseTrackerDbContext context)
    {
        await context.Database.MigrateAsync();
        if (await context.Categories.AnyAsync())
            return;
        var categories = new List<Category>
        {
            new() { Name = "Food" },
            new() { Name = "Transportation" },
            new() { Name = "Entertainment" },
            new() { Name = "Housing" },
            new() { Name = "Utilities" },
            new() { Name = "Shopping" },
            new() { Name = "Other" }
        };
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
    }
}