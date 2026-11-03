using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostExpense_Valid_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/expenses",
            new { description = "Coffee", amount = 4.5, category = "Food" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostExpense_Invalid_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/expenses",
            new { description = "", amount = -1, category = "Food" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Summary_ReturnsOk()
    {
        var response = await _client.GetAsync("/summary");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
