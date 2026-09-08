using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Api.Models;

public class Expense
{
    public int Id { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateOnly ExpenseDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Foreign key
    [Required]
    public int CategoryId { get; set; }

    //Navigation property
    public Category? Category { get; set; }

    //Foreign key    
    [Required]
    public int UserId { get; set; }

    //Navigation property
    public User? User { get; set; }
}