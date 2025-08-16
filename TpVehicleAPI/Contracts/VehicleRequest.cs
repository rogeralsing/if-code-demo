namespace TpVehicleAPI;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public readonly record struct VehicleRequest(
    [FromRoute] [StringLength(7, MinimumLength = 2)] string RegistrationNumber);
