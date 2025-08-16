namespace TpInsuranceAPI;

public interface IInsuranceDataProvider
{
    Task<IEnumerable<Insurance>> GetInsurancesAsync(string personalNumber);
}

public sealed class HardcodedInsuranceDataProvider : IInsuranceDataProvider
{
    private static readonly Dictionary<string, Insurance[]> Data = new()
    {
        ["190101011234"] =
        [
            new Insurance(InsuranceType.Car, "ABC123"),
            new Insurance(InsuranceType.PersonalHealth, null)
        ],
        ["197705055678"] =
        [
            new Insurance(InsuranceType.Pet, null)
        ]
    };

    public Task<IEnumerable<Insurance>> GetInsurancesAsync(string personalNumber)
    {
        Data.TryGetValue(personalNumber, out var insurances);
        return Task.FromResult(insurances?.AsEnumerable() ?? Enumerable.Empty<Insurance>());
    }
}
