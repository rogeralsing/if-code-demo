namespace TpVehicleAPI;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MiniValidation;

public static class VehicleEndpoints
{
    public static IEndpointRouteBuilder MapVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/vehicles/{registrationNumber}",
                async ([AsParameters] VehicleRequest request, IVehicleDataProvider provider) =>
                {
                    if (!MiniValidator.TryValidate(request, out var errors))
                        return Results.ValidationProblem(errors);

                    var vehicle = await provider.GetVehicleAsync(request.RegistrationNumber);
                    return vehicle is not null
                        ? Results.Ok(vehicle)
                        : Results.Problem(statusCode: 404, title: $"Vehicle {request.RegistrationNumber} not found");
                })
            .RequireRateLimiting("fixed")
            .WithName("GetVehicle")
            .WithOpenApi();

        return app;
    }
}
