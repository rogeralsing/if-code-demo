namespace TpVehicleAPI;

public interface IVehicleDataProvider
{
    Task<VehicleResponse?> GetVehicleAsync(string registrationNumber);
}

public sealed class HardcodedVehicleDataProvider : IVehicleDataProvider
{
    private static readonly VehicleResponse[] Vehicles =
    {
        new("ABC123", "Volvo", "V70", 2016),
        new("DEF456", "Saab", "9-3", 2008),
        new("GHI789", "Tesla", "Model 3", 2023)
    };

    public Task<VehicleResponse?> GetVehicleAsync(string registrationNumber)
    {
        var match = Vehicles.FirstOrDefault(v =>
            string.Equals(v.RegistrationNumber, registrationNumber, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(match);
    }
}
