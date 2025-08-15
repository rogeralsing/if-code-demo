using TpInsuranceAPI.Models;

namespace TpInsuranceAPI.Providers;

/// <summary>
/// Simple in-memory provider with hardcoded insurance data.
/// </summary>
public class HardcodedCarInsuranceProvider : ICarInsuranceProvider
{
    private static readonly Dictionary<string, CarInsurance> _insurances = new()
    {
        ["ABC123"] = new("ABC123", 30m),
        ["DEF456"] = new("DEF456", 30m)
    };

    public Task<CarInsurance?> GetAsync(string registrationNumber)
    {
        _insurances.TryGetValue(registrationNumber.ToUpperInvariant(), out var insurance);
        return Task.FromResult(insurance);
    }
}
