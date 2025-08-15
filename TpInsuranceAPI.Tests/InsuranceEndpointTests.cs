using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TpInsuranceAPI;

namespace TpInsuranceAPI.Tests;

// Tests for the insurance endpoint
public class InsuranceEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public InsuranceEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
                builder.UseEnvironment("Testing")
                    .ConfigureServices(services =>
                    {
                        services.RemoveAll<IInsuranceDataProvider>();
                        services.AddSingleton<IInsuranceDataProvider, TestInsuranceDataProvider>();
                        services.RemoveAll<IVehicleClient>();
                        services.AddSingleton<IVehicleClient, TestVehicleClient>();
                    }))
            .CreateClient();
    }

    [Fact]
    public async Task ReturnsInsurancesForKnownPersonalNumber()
    {
        var insurances = await _client.GetFromJsonAsync<InsuranceResponse[]>("/insurances/190101011234", Json);
        Assert.NotNull(insurances);
        var car = insurances!.First(i => i.Type == InsuranceType.Car);
        Assert.NotNull(car.Vehicle);
        Assert.Equal("TST123", car.Vehicle!.RegistrationNumber);
        Assert.Equal(30m, car.MonthlyCost);
        var pet = insurances!.First(i => i.Type == InsuranceType.Pet);
        Assert.Equal(10m, pet.MonthlyCost);
    }

    [Fact]
    public async Task ReturnsNotFoundForUnknownPersonalNumber()
    {
        var response = await _client.GetAsync("/insurances/200001010000");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("No insurances found", problem!.Title);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidPersonalNumber()
    {
        var response = await _client.GetAsync("/insurances/123");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("PersonalNumber", problem!.Errors.Keys);
    }

    [Fact]
    public async Task ReturnsConflictWhenMultiplePersonalHealthInsurances()
    {
        var response = await _client.GetAsync("/insurances/199901019999");
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Multiple personal health insurances found", problem!.Title);
    }

    [Fact]
    public async Task ReturnsNotFoundWhenVehicleMissing()
    {
        var response = await _client.GetAsync("/insurances/190101011235");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Vehicle MISS123 not found", problem!.Title);
    }
}
