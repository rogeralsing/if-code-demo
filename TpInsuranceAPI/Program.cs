using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// simple hardcoded services
builder.Services.AddSingleton<IInsuranceDataProvider, HardcodedInsuranceDataProvider>();
builder.Services.AddHttpClient<IVehicleClient, VehicleApiClient>(client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("VehicleApiBaseUrl") ?? "http://localhost:5005";
    client.BaseAddress = new Uri(baseUrl);
});

// Serialize enums as strings in JSON responses
builder.Services.ConfigureHttpJsonOptions(opts =>
    opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

if (app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/insurances/{personalNumber}", async ([AsParameters] PersonRequest request, IInsuranceDataProvider provider, IVehicleClient vehicleClient) =>
    {
        var context = new ValidationContext(request);
        // Minimal APIs do not trigger DataAnnotations automatically
        if (!Validator.TryValidateObject(request, context, new List<ValidationResult>(), true))
            return Results.BadRequest();

        var insurances = (await provider.GetInsurancesAsync(request.PersonalNumber)).ToArray();
        if (insurances.Length == 0)
            return Results.NotFound();

        var responses = new List<InsuranceResponse>();
        foreach (var insurance in insurances)
        {
            VehicleResponse? vehicle = null;
            if (!string.IsNullOrEmpty(insurance.RegistrationNumber))
            {
                vehicle = await vehicleClient.GetVehicleAsync(insurance.RegistrationNumber);
            }
            responses.Add(new InsuranceResponse(insurance.Type, insurance.MonthlyCost, vehicle));
        }

        return Results.Ok(responses);
    })
    .WithName("GetInsurances")
    .WithOpenApi();

app.Run();

// Data provider abstraction
public interface IInsuranceDataProvider
{
    Task<IEnumerable<Insurance>> GetInsurancesAsync(string personalNumber);
}

// Hardcoded provider for demo purposes
public sealed class HardcodedInsuranceDataProvider : IInsuranceDataProvider
{
    private static readonly Dictionary<string, Insurance[]> Data = new()
    {
        ["190101011234"] =
        [
            new Insurance(InsuranceType.Car, "ABC123"),
            new Insurance(InsuranceType.PersonalHealth, null)
        ],
        ["197705055678"] =
        [
            new Insurance(InsuranceType.Pet, null)
        ]
    };

    public Task<IEnumerable<Insurance>> GetInsurancesAsync(string personalNumber)
    {
        Data.TryGetValue(personalNumber, out var insurances);
        return Task.FromResult(insurances?.AsEnumerable() ?? Enumerable.Empty<Insurance>());
    }
}

// Client for vehicle lookup
public interface IVehicleClient
{
    Task<VehicleResponse?> GetVehicleAsync(string registrationNumber);
}

public sealed class VehicleApiClient(HttpClient httpClient) : IVehicleClient
{
    public async Task<VehicleResponse?> GetVehicleAsync(string registrationNumber)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<VehicleResponse>($"/vehicles/{registrationNumber}");
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return null; // missing vehicle information is allowed
        }
    }
}

// Request and response models
public sealed class PersonRequest
{
    [FromRoute]
    [StringLength(12, MinimumLength = 10)]
    public required string PersonalNumber { get; init; }
}

public sealed record InsuranceResponse(InsuranceType Type, decimal MonthlyCost, VehicleResponse? Vehicle);

public sealed record Insurance(InsuranceType Type, string? RegistrationNumber)
{
    public decimal MonthlyCost => Type switch
    {
        InsuranceType.Pet => 10m,
        InsuranceType.PersonalHealth => 20m,
        InsuranceType.Car => 30m,
        _ => 0m
    };
}

public sealed record VehicleResponse(string RegistrationNumber, string Manufacturer, string Model, int ModelYear);

// A simple enum is sufficient; the set of supported insurances is small and bounded,
// so polymorphism would add unnecessary complexity.
public enum InsuranceType
{
    Car,
    Pet,
    PersonalHealth
}

public partial class Program;
