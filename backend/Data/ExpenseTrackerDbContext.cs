using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Data;

public class ExpenseTrackerDbContext : DbContext
{
    public ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Expense>()
            .Property(e => e.ExpenseDate)
            .HasColumnType("date");
    }
}