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
        var insurances = await _client.GetFromJsonAsync<InsuranceResponse[]>("/api/v1/insurances/190101011234", Json);
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
        var response = await _client.GetAsync("/api/v1/insurances/200001010000");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("No insurances found", problem!.Title);
    }

    [Fact]
    public async Task ReturnsBadRequestForInvalidPersonalNumber()
    {
        var response = await _client.GetAsync("/api/v1/insurances/123");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("PersonalNumber", problem!.Errors.Keys);
    }

    [Fact]
    public async Task ReturnsConflictWhenMultiplePersonalHealthInsurances()
    {
        var response = await _client.GetAsync("/api/v1/insurances/199901019999");
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Multiple personal health insurances found", problem!.Title);
    }

    [Fact]
    public async Task ReturnsNotFoundWhenVehicleMissing()
    {
        var response = await _client.GetAsync("/api/v1/insurances/190101011235");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Vehicle MISS123 not found", problem!.Title);
}

    [Fact]
    public async Task ReturnsServiceUnavailableWhenVehicleServiceFails()
    {
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseEnvironment("Testing").ConfigureServices(services =>
            {
                services.RemoveAll<IInsuranceDataProvider>();
                services.AddSingleton<IInsuranceDataProvider, TestInsuranceDataProvider>();
                services.RemoveAll<IVehicleClient>();
                services.AddSingleton<IVehicleClient, FailingVehicleClient>();
            }));

        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/v1/insurances/190101011234");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Vehicle service unavailable", problem!.Title);
    }

    private sealed class FailingVehicleClient : IVehicleClient
    {
        public Task<VehicleResponse?> GetVehicleAsync(string registrationNumber)
            => throw new VehicleServiceException("fail", new Exception());
    }
}
