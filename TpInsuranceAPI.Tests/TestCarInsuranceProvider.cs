using TpInsuranceAPI.Models;
using TpInsuranceAPI.Providers;

namespace TpInsuranceAPI.Tests;

/// <summary>
/// Minimal provider used for tests so data lives in the test project.
/// </summary>
public class TestCarInsuranceProvider : ICarInsuranceProvider
{
    private readonly Dictionary<string, CarInsurance> _data;

    public TestCarInsuranceProvider(Dictionary<string, CarInsurance> data)
    {
        _data = data;
    }

    public Task<CarInsurance?> GetAsync(string registrationNumber)
    {
        _data.TryGetValue(registrationNumber.ToUpperInvariant(), out var insurance);
        return Task.FromResult(insurance);
    }
}

