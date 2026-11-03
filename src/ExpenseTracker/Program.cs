using ExpenseTracker;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ExpenseService>();
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/expenses", (ExpenseService service) => service.GetAll());

app.MapPost("/expenses", (NewExpense request, ExpenseService service) =>
{
    try
    {
        var expense = service.Add(request.Description, request.Amount, request.Category);
        return Results.Created($"/expenses/{expense.Id}", expense);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/summary", (ExpenseService service) =>
    new { total = service.Total(), byCategory = service.TotalsByCategory() });

app.Run();

public record NewExpense(string Description, decimal Amount, string Category);

public partial class Program { }
