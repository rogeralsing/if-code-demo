namespace TpInsuranceAPI;

using System.Net;
using System.Net.Http.Json;

public interface IVehicleClient
{
    Task<VehicleResponse?> GetVehicleAsync(string registrationNumber);
}

public sealed class VehicleApiClient(HttpClient httpClient) : IVehicleClient
{
    public async Task<VehicleResponse?> GetVehicleAsync(string registrationNumber)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<VehicleResponse>($"/api/v1/vehicles/{registrationNumber}");
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (HttpRequestException e)
        {
            throw new VehicleServiceException("Vehicle service failure", e);
        }
    }
}

public sealed class VehicleServiceException(string message, Exception inner) : Exception(message, inner);
