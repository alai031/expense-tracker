namespace ExpenseTracker.Api.Models;

public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly ExpenseDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Foreign key
    public int CategoryId { get; set; }
    //Navigation property
    public Category? Category { get; set; }
    //Foreign key    
    public int UserId { get; set; }
    //Navigation property
    public User? User { get; set; }
}