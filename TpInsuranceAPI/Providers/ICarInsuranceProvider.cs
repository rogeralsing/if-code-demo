using TpInsuranceAPI.Models;

namespace TpInsuranceAPI.Providers;

/// <summary>
/// Abstraction for fetching car insurance data.
/// </summary>
public interface ICarInsuranceProvider
{
    Task<CarInsurance?> GetAsync(string registrationNumber);
}
