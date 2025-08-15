using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TpInsuranceAPI.Models;
using TpInsuranceAPI.Providers;
using Xunit;

namespace TpInsuranceAPI.Tests;

public class CarInsuranceEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CarInsuranceEndpointTests(WebApplicationFactory<Program> factory)
    {
        // Test-specific data lives here to decouple tests from production data
        var data = new Dictionary<string, CarInsurance>
        {
            ["TST123"] = new("TST123", 40m)
        };

        var configuredFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ICarInsuranceProvider>();
                services.AddSingleton<ICarInsuranceProvider>(new TestCarInsuranceProvider(data));
            });
        });

        _client = configuredFactory.CreateClient();
    }

    [Fact]
    public async Task Known_car_returns_insurance()
    {
        var response = await _client.PostAsJsonAsync("/car-insurance", new CarInsuranceRequest("TST123"));
        var insurance = await response.Content.ReadFromJsonAsync<CarInsuranceResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(insurance);
        Assert.Equal("TST123", insurance!.RegistrationNumber);
    }

    [Fact]
    public async Task Unknown_car_returns_not_found()
    {
        var response = await _client.PostAsJsonAsync("/car-insurance", new CarInsuranceRequest("ZZZ999"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_plate_returns_bad_request()
    {
        var response = await _client.PostAsJsonAsync("/car-insurance", new CarInsuranceRequest("BADPLATE"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
