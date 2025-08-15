using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TpVehicleAPI.Tests;

// Ensures the API host can be started
public class WeatherEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WeatherEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void FactoryCreatesClient()
    {
        var client = _factory.CreateClient();
        Assert.NotNull(client);
    }
}
