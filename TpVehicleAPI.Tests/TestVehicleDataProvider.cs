using TpVehicleAPI;

namespace TpVehicleAPI.Tests;

// Simple data provider used only for tests
public sealed class TestVehicleDataProvider : IVehicleDataProvider
{
    private static readonly VehicleResponse[] Vehicles =
    {
        new("TST123", "TestBrand", "TestModel", 2024)
    };

    public Task<VehicleResponse?> GetVehicleAsync(string registrationNumber)
    {
        var match = Vehicles.FirstOrDefault(v =>
            string.Equals(v.RegistrationNumber, registrationNumber, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(match);
    }
}
