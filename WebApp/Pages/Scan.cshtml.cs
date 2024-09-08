using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Albstones.WebApp.Pages;

public class ScanModel : PageModel
{
    private readonly ILogger<ScanModel> _logger;

    public ScanModel(ILogger<ScanModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}
