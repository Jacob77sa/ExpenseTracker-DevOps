using ExpenseTracker;

var startedAt = DateTime.UtcNow;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ExpenseService>();
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/status", (ExpenseService service) => new
{
    status = "healthy",
    startedAt,
    uptimeSeconds = (long)(DateTime.UtcNow - startedAt).TotalSeconds,
    expenses = service.GetAll().Count,
    total = service.Total()
});

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
