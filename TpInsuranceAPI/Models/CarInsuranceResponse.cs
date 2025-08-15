namespace TpInsuranceAPI.Models;

/// <summary>
/// Response returned from the car insurance lookup.
/// </summary>
public record CarInsuranceResponse(string RegistrationNumber, decimal MonthlyCost);
