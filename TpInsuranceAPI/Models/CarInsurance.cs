namespace TpInsuranceAPI.Models;

/// <summary>
/// Basic car insurance information returned to callers.
/// </summary>
public record CarInsurance(string RegistrationNumber, decimal MonthlyCost);
