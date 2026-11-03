namespace ExpenseTracker;

public record Expense(int Id, string Description, decimal Amount, string Category);

public class ExpenseService
{
    private readonly List<Expense> _items = new();
    private readonly object _lock = new();

    public Expense Add(string description, decimal amount, string category)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.");
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category is required.");

        lock (_lock)
        {
            var expense = new Expense(_items.Count + 1, description.Trim(), amount, category.Trim());
            _items.Add(expense);
            return expense;
        }
    }

    public IReadOnlyList<Expense> GetAll()
    {
        lock (_lock) return _items.ToList();
    }

    public Dictionary<string, decimal> TotalsByCategory()
    {
        lock (_lock)
            return _items
                .GroupBy(e => e.Category, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount), StringComparer.OrdinalIgnoreCase);
    }

    public decimal Total()
    {
        lock (_lock) return _items.Sum(e => e.Amount);
    }
}
