namespace TpInsuranceAPI;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MiniValidation;

public static class InsuranceEndpoints
{
    public static IEndpointRouteBuilder MapInsuranceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/insurances/{personalNumber}",
                async ([AsParameters] PersonRequest request, IInsuranceDataProvider provider, IVehicleClient vehicleClient) =>
                {
                    if (!MiniValidator.TryValidate(request, out var errors))
                        return Results.ValidationProblem(errors);

                    var insurances = (await provider.GetInsurancesAsync(request.PersonalNumber)).ToArray();
                    if (insurances.Length == 0)
                        return Results.Problem(statusCode: 404, title: "No insurances found");

                    if (insurances.Count(i => i.Type == InsuranceType.PersonalHealth) > 1)
                        return Results.Problem(statusCode: 409, title: "Multiple personal health insurances found");

                    var responses = new List<InsuranceResponse>();
                    foreach (var insurance in insurances)
                    {
                        VehicleResponse? vehicle = null;
                        if (!string.IsNullOrEmpty(insurance.RegistrationNumber))
                        {
                            try
                            {
                                vehicle = await vehicleClient.GetVehicleAsync(insurance.RegistrationNumber);
                            }
                            catch (VehicleServiceException)
                            {
                                return Results.Problem(statusCode: 503, title: "Vehicle service unavailable");
                            }

                            if (vehicle is null)
                                return Results.Problem(statusCode: 404, title: $"Vehicle {insurance.RegistrationNumber} not found");
                        }
                        responses.Add(new InsuranceResponse(insurance.Type, insurance.MonthlyCost, vehicle));
                    }

                    return Results.Ok(responses);
                })
            .RequireRateLimiting("fixed")
            .WithName("GetInsurances")
            .WithOpenApi();

        return app;
    }
}
