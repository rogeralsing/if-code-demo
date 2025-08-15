using TpInsuranceAPI;

namespace TpInsuranceAPI.Tests;

// Simple data provider used for tests
public sealed class TestInsuranceDataProvider : IInsuranceDataProvider
{
    private static readonly Dictionary<string, Insurance[]> Data = new()
    {
        ["190101011234"] =
        [
            new Insurance(InsuranceType.Car, "TST123"),
            new Insurance(InsuranceType.Pet, null)
        ]
    };

    public Task<IEnumerable<Insurance>> GetInsurancesAsync(string personalNumber)
    {
        Data.TryGetValue(personalNumber, out var insurances);
        return Task.FromResult(insurances?.AsEnumerable() ?? Enumerable.Empty<Insurance>());
    }
}
