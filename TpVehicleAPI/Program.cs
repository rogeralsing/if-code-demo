using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IVehicleDataProvider, HardcodedVehicleDataProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/vehicles/{registrationNumber}", async ([AsParameters] VehicleRequest request, IVehicleDataProvider provider) =>
    {
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();
        // minimal APIs don't run DataAnnotations automatically, so validate manually
        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            var errors = validationResults
                .SelectMany(r => r.MemberNames.Select(m => (m, error: r.ErrorMessage ?? "Invalid value")))
                .GroupBy(e => e.m)
                .ToDictionary(g => g.Key, g => g.Select(e => e.error).ToArray());
            return Results.ValidationProblem(errors);
        }

        var plate = request.RegistrationNumber;
        var vehicle = await provider.GetVehicleAsync(plate);
        return vehicle is not null
            ? Results.Text(JsonSerializer.Serialize(vehicle), "application/json")
            : Results.Problem(statusCode: 404, title: $"Vehicle {plate} not found");
    })
    .WithName("GetVehicle")
    .WithOpenApi();

app.Run();

public interface IVehicleDataProvider
{
    Task<VehicleResponse?> GetVehicleAsync(string registrationNumber);
}

public sealed class HardcodedVehicleDataProvider : IVehicleDataProvider
{
    private static readonly VehicleResponse[] Vehicles =
    {
        new("ABC123", "Volvo", "V70", 2016),
        new("DEF456", "Saab", "9-3", 2008),
        new("GHI789", "Tesla", "Model 3", 2023)
    };

    public Task<VehicleResponse?> GetVehicleAsync(string registrationNumber)
    {
        var match = Vehicles.FirstOrDefault(v =>
            string.Equals(v.RegistrationNumber, registrationNumber, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(match);
    }
}

public sealed class VehicleRequest
{
    [FromRoute]
    [StringLength(7, MinimumLength = 2)]
    public required string RegistrationNumber { get; init; }
}

public sealed record VehicleResponse(string RegistrationNumber, string Manufacturer, string Model, int ModelYear);

public partial class Program;
