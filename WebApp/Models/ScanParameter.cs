using System.ComponentModel.DataAnnotations;

namespace Albstones.WebApp.Models;

public class ScanParameter
{
    [RegularExpression(@"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/][AQgw]==|[A-Za-z0-9+/]{2}[AEIMQUYcgkosw048]=)?$", ErrorMessage = "Invalid scan parameter")] // base64
    public required string Code { get; set; }
}
