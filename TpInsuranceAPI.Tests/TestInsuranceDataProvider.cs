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
        ],
        ["190101011235"] =
        [
            new Insurance(InsuranceType.Car, "MISS123")
        ],
        ["199901019999"] =
        [
            new Insurance(InsuranceType.PersonalHealth, null),
            new Insurance(InsuranceType.PersonalHealth, null)
        ]
    };

    public Task<IEnumerable<Insurance>> GetInsurancesAsync(string personalNumber)
    {
        Data.TryGetValue(personalNumber, out var insurances);
        return Task.FromResult(insurances?.AsEnumerable() ?? Enumerable.Empty<Insurance>());
    }
}
