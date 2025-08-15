using Microsoft.AspNetCore.Mvc;
using TpInsuranceAPI.Models;
using TpInsuranceAPI.Providers;

namespace TpInsuranceAPI.Controllers;

/// <summary>
/// Exposes car insurance information for a given registration number.
/// </summary>
[ApiController]
[Route("car-insurance")]
public class CarInsuranceController : ControllerBase
{
    private readonly ICarInsuranceProvider _provider;

    public CarInsuranceController(ICarInsuranceProvider provider) => _provider = provider;

    [HttpPost]
    public async Task<ActionResult<CarInsuranceResponse>> GetInsurance(CarInsuranceRequest request)
    {
        var insurance = await _provider.GetAsync(request.RegistrationNumber);
        if (insurance is null)
        {
            return NotFound();
        }

        return Ok(new CarInsuranceResponse(insurance.RegistrationNumber, insurance.MonthlyCost));
    }
}
