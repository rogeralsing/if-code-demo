using TpInsuranceAPI;

namespace TpInsuranceAPI.Tests;

// Fake vehicle client used for tests
public sealed class TestVehicleClient : IVehicleClient
{
    public Task<VehicleResponse?> GetVehicleAsync(string registrationNumber)
    {
        return Task.FromResult<VehicleResponse?>(
            registrationNumber == "TST123"
                ? new VehicleResponse("TST123", "TestBrand", "TestModel", 2024)
                : null);
    }
}
