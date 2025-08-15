using System.ComponentModel.DataAnnotations;

namespace TpInsuranceAPI.Models;

/// <summary>
/// Request body for looking up car insurance by registration number.
/// </summary>
public record CarInsuranceRequest(
    [Required]
    [RegularExpression("^[A-HJ-PR-UW-Z]{3}[0-9]{2}[0-9A-HJ-PR-UW-Z]$",
        ErrorMessage = "Registration number must follow Swedish format")]
    string RegistrationNumber);
