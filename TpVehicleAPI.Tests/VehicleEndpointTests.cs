using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TpVehicleAPI;

namespace TpVehicleAPI.Tests;

// Basic tests for the vehicle endpoint
public class VehicleEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public VehicleEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
            builder.UseEnvironment("Testing").UseKestrel()
                .ConfigureServices(services =>
                {
                    services.RemoveAll<IVehicleDataProvider>();
                    services.AddSingleton<IVehicleDataProvider, TestVehicleDataProvider>();
                })).CreateClient();
    }

    [Fact]
    public async Task ReturnsVehicleForKnownRegistration()
    {
        var vehicle = await _client.GetFromJsonAsync<VehicleResponse>("/vehicles/TST123");
        Assert.NotNull(vehicle);
        Assert.Equal("TST123", vehicle!.RegistrationNumber);
    }

    [Fact]
    public async Task ReturnsNotFoundForUnknownRegistration()
    {
        var response = await _client.GetAsync("/vehicles/NOPE999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidRegistration()
    {
        var response = await _client.GetAsync("/vehicles/A");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
