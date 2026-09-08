using ExpenseTracker.Api.Controllers;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Tests;

public class ExpenseControllerTests
{
    private ExpenseTrackerDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new ExpenseTrackerDbContext(options);
    }

    private void SetUser(ExpensesController controller, int userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [Fact]
    public async Task CreateExpense_ValidExpense_CreatesExpense()
    {
        //Arrange
        var context = CreateDbContext();
        context.Categories.Add(new Category
        {
            Id = 1,
            Name = "Food"
        });
        await context.SaveChangesAsync();
        var controller = new ExpensesController(context);
        SetUser(controller, 1);
        var expense = new Expense
        {
            Amount = 50,
            Description = "Dinner",
            ExpenseDate = new DateOnly(2026, 9, 7),
            CategoryId = 1
        };

        //Act
        var result = await controller.CreateExpense(expense);

        //Assert
        var createdExpense = await context.Expenses.FirstOrDefaultAsync();

        Assert.NotNull(createdExpense);
        Assert.Equal(50, createdExpense.Amount);
        Assert.Equal("Dinner", createdExpense.Description);
        Assert.Equal(1, createdExpense.UserId);
    }

    [Fact]
    public async Task CreateExpense_InvalidAmount_ReturnsBadRequest()
    {
        //Arrange
        var context = CreateDbContext();
        context.Categories.Add(new Category
        {
            Id = 1,
            Name = "Food"
        });
        await context.SaveChangesAsync();
        var controller = new ExpensesController(context);
        SetUser(controller, 1);
        var expense = new Expense
        {
            Amount = -10,
            Description = "Dinner",
            ExpenseDate = new DateOnly(2026, 9, 7),
            CategoryId = 1
        };

        //Act
        var result = await controller.CreateExpense(expense);

        //Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Amount must be greater than 0.", badRequest.Value);
    }

    [Fact]
    public async Task DeleteExpense_ExpenseDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        var context = CreateDbContext();
        var controller = new ExpensesController(context);
        SetUser(controller, 1);

        //Act
        var result = await controller.DeleteExpense(999);

        //Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Expense not found.", notFound.Value);
    }

    [Fact]
    public async Task DeleteExpense_UserCannotDeleteAnotherUsersExpense_ReturnsNotFound()
    {
        //Arrange
        var context = CreateDbContext();
        context.Expenses.Add(new Expense
        {
            Id = 2,
            Amount = 50,
            Description = "User 2 expense",
            ExpenseDate = new DateOnly(2026, 9, 7),
            CategoryId = 1,
            UserId = 2
        });
        await context.SaveChangesAsync();
        var controller = new ExpensesController(context);
        //Pretend we're User 1
        SetUser(controller, 1);

        //Act
        var result = await controller.DeleteExpense(2);

        //Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Expense not found.", notFound.Value);
    }
}