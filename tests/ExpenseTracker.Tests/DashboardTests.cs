using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class DashboardTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DashboardTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Root_ServesDashboardPage()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Status_ReturnsOk()
    {
        var response = await _client.GetAsync("/status");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
