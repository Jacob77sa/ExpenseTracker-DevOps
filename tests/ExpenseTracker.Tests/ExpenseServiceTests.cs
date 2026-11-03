using ExpenseTracker;
using Xunit;

public class ExpenseServiceTests
{
    [Fact]
    public void Add_ValidExpense_IsStored()
    {
        var service = new ExpenseService();
        var expense = service.Add("Coffee", 4.5m, "Food");

        Assert.Equal(1, expense.Id);
        Assert.Single(service.GetAll());
    }

    [Theory]
    [InlineData("", 10, "Food")]
    [InlineData("Lunch", 0, "Food")]
    [InlineData("Lunch", -5, "Food")]
    [InlineData("Lunch", 10, " ")]
    public void Add_InvalidInput_Throws(string description, double amount, string category)
    {
        var service = new ExpenseService();
        Assert.Throws<ArgumentException>(() => service.Add(description, (decimal)amount, category));
    }

    [Fact]
    public void Totals_GroupByCategory_IgnoringCase()
    {
        var service = new ExpenseService();
        service.Add("Lunch", 10m, "Food");
        service.Add("Dinner", 15m, "food");
        service.Add("Bus", 2m, "Transport");

        var totals = service.TotalsByCategory();

        Assert.Equal(25m, totals["Food"]);
        Assert.Equal(2m, totals["Transport"]);
        Assert.Equal(27m, service.Total());
    }
}
