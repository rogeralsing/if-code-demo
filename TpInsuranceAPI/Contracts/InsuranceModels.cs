namespace TpInsuranceAPI;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public readonly record struct PersonRequest(
    [FromRoute] [StringLength(12, MinimumLength = 10)] string PersonalNumber);

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

public enum InsuranceType
{
    Car,
    Pet,
    PersonalHealth
}
